using SmsTools.Commands;
using SmsTools.Operations;
using SmsTools.PduProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmsTools
{
    public class MessageStorage
    {
        private PduProfileManager _manager = new PduProfileManager();
        private ICommandParameter _empty = null;
        private IATCommand _storageQuery = null;
        private IATCommand _listQuery = null;
        private IATCommand _mfCmd = null;


        public MessageStorage()
        {
            init();
        }

        private void init()
        {
            _empty = CommandParameter.CreateEmpty(Constants.BasicSuccessfulResponse);
            _storageQuery = new SimpleATCommand(ATCommand.MessageStorageInfo.Command(), _empty);

            var listParam = new CommandParameter(Constants.MessageStatus.Any.ToValueString(), Constants.BasicSuccessfulResponse);
            _listQuery = new ParamATCommand(ATCommand.MessageList.Command(), listParam);

            var mfParam = new CommandParameter(Constants.MessageFormat.Pdu.ToValueString(), Constants.BasicSuccessfulResponse);
            _mfCmd = new ParamATCommand(ATCommand.MessageFormat.Command(), mfParam);
        }

        public async Task<bool> SelectStorage(IPortPlug port, Constants.MessageStorage readStore = Constants.MessageStorage.MobileEquipment, Constants.MessageStorage writeStore = Constants.MessageStorage.Unspecified, Constants.MessageStorage receivedStore = Constants.MessageStorage.Unspecified)
        {
            if (readStore == Constants.MessageStorage.Unspecified)
                return false;

            var storageParam = new CommandParameter(getStorageParam(readStore, writeStore, receivedStore), Constants.BasicSuccessfulResponse);
            var storageCmd = new ParamATCommand(ATCommand.MessageStorage.Command(), storageParam);

            await storageCmd.ExecuteAsync(port);

            return storageCmd.Succeeded();
        }

        public async Task<IEnumerable<MessageStorageState>> StorageState(IPortPlug port)
        {
            var result = Enumerable.Empty<MessageStorageState>();

            await _storageQuery.ExecuteAsync(port);
            if (_storageQuery.Succeeded())
            {
                result = getStorageState(_storageQuery.Response);
            }

            return result;
        }

        public async Task<IEnumerable<MessageStorageItem>> List(IPortPlug port)
        {
            var result = Enumerable.Empty<MessageStorageItem>();

            if (await setFormat(port))
            {
                await _listQuery.ExecuteAsync(port);
                if (_listQuery.Succeeded())
                {
                    result = getStorageItems(_listQuery.Response);
                }
            }

            return result;
        }

        public async Task<MessageDetails> Read(IPortPlug port, MessageStorageItem item)
        {
            MessageDetails result = new MessageDetails();

            if (item == null || !item.IsValid || !_manager.ContainsProfile("default-receive"))
                return result;

            if (await setFormat(port))
            {
                var readParam = new CommandParameter(item.Index.ToString(), Constants.BasicSuccessfulResponse);
                var readCmd = new ParamATCommand(ATCommand.MessageRead.Command(), readParam);

                await readCmd.ExecuteAsync(port);
                if (readCmd.Succeeded())
                {
                    result = getMessage(readCmd.Response);
                }
            }

            return result;
        }

        public async Task<bool> Delete(IPortPlug port, MessageStorageItem item, DeleteFlag deleteFlag = DeleteFlag.SpecifiedByIndex)
        {
            bool result = false;

            if (item == null || !item.IsValid)
                return result;

            if (await setFormat(port))
            {
                var deleteParam = new CommandParameter($"{item.Index},{deleteFlag.ToValueString()}", Constants.BasicSuccessfulResponse);
                var deleteCmd = new ParamATCommand(ATCommand.MessageDelete.Command(), deleteParam);

                await deleteCmd.ExecuteAsync(port);
                result = deleteCmd.Succeeded();
            }

            return result;
        }


        private MessageDetails getMessage(string response)
        {
            MessageDetails result = new MessageDetails();

            try
            {
                var lengthMatch = Regex.Match(response, @"^\S*(?:cmgr:.+,(\d+)\s*)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                var pduMatch = Regex.Match(response, @"^(?:\s*([a-fA-F0-9]+)\s*)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);

                if (lengthMatch.Success && lengthMatch.Groups.Count > 1 && pduMatch.Success && pduMatch.Groups.Count > 1)
                {
                    int length = int.Parse(lengthMatch.Groups[1].Value.Trim());
                    string pdu = pduMatch.Groups[1].Value.Trim();

                    var profile = _manager["default-receive"];

                    result = profile.GetMessage(pdu, length);
                }
            }
            catch { }

            return result;
        }

        private IEnumerable<MessageStorageItem> getStorageItems(string response)
        {
            var result = Enumerable.Empty<MessageStorageItem>();

            try
            {
                var matches = Regex.Matches(response, @"^\S*(?:cmgl:(.+))$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (matches.Count > 0)
                {
                    var items = new List<MessageStorageItem>();

                    for (int m = 0; m < matches.Count; ++m)
                    {
                        var match = matches[m];
                        if (!match.Success || match.Groups.Count < 2)
                            throw new Exception();

                        var itemValue = match.Groups[1].Value.Trim();
                        var itemValues = itemValue.Split(',');
                        int value = 0;
                        if (itemValues.Length < 3 || itemValues.Any(v => !int.TryParse(v, out value)))
                            throw new Exception();

                        var item = itemValues.Select(v => int.Parse(v)).ToArray();

                        items.Add(new MessageStorageItem() { Index = item[0], Status = (Constants.MessageStatus)item[1], Length = item.Last() });
                    }

                    result = items;
                }
            }
            catch { }

            return result;
        }

        private IEnumerable<MessageStorageState> getStorageState(string response)
        {
            var result = Enumerable.Empty<MessageStorageState>();

            try
            {
                var state = Regex.Match(response, $@"(?:cpms:(.+){Constants.BasicSuccessfulResponse})", RegexOptions.IgnoreCase);
                if (state.Success && state.Groups.Count > 1)
                {
                    var stateValue = state.Groups[1].Value.Trim();
                    var stateValues = stateValue.Split(',');
                    if (stateValues.Length % 3 == 0 && stateValues.All(s => !string.IsNullOrWhiteSpace(s)))
                    {
                        var storages = new List<MessageStorageState>();

                        for (int i = 0; i < stateValues.Length; i += 3)
                        {
                            var storageValue = stateValues[i];
                            var countValue = stateValues[i + 1];
                            var limitValue = stateValues[i + 2];

                            int count = 0, limit = 0;
                            if (!int.TryParse(countValue, out count) || !int.TryParse(limitValue, out limit))
                                throw new Exception();

                            storages.Add(new MessageStorageState() { Storage = storageValue.ToMessageStorage(), Count = count, Limit = limit });
                        }

                        result = storages;
                    }
                }
            }
            catch { }

            return result;
        }

        private string getStorageParam(Constants.MessageStorage readStore, Constants.MessageStorage writeStore, Constants.MessageStorage receivedStore)
        {
            var param = new StringBuilder(readStore.Description());

            if (writeStore != Constants.MessageStorage.Unspecified)
            {
                param.Append(",").Append(writeStore.Description());
            }

            if (receivedStore != Constants.MessageStorage.Unspecified)
            {
                param.Append(",").Append(receivedStore.Description());
            }

            return param.ToString();
        }

        private async Task<bool> setFormat(IPortPlug port)
        {
            await _mfCmd.ExecuteAsync(port);
            return _mfCmd.Succeeded();
        }
    }


    public class MessageStorageState
    {
        public Constants.MessageStorage Storage { get; set; }
        public int Count { get; set; }
        public int Limit { get; set; }
        public bool IsFull { get { return Count == Limit; } }
        public bool IsEmpty { get { return Count == 0; } }
    }

    public class MessageStorageItem
    {
        public int Index { get; set; }
        public Constants.MessageStatus Status { get; set; }
        public int Length { get; set; }
        public bool IsValid { get { return Index >= 0 && Length >= 0; } }
    }

    public enum DeleteFlag
    {
        SpecifiedByIndex,
        Read,
        ReadAndSent,
        ExcludeUnread,
        All
    }
}
