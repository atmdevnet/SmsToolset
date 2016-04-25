using SmsTools;
using SmsTools.Commands;
using SmsTools.Operations;
using SmsTools.PduProfile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            //int length = 0;
            //string packet = string.Empty;

            //var p = new PduProfileManager();
            //if (p.HasDefaultProfile)
            //{
            //    length = 0;
            //    packet = p.DefaultProfile.GetPacket(48783314087, "华为", out length);
            //}

            //using (var ps = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(Program), "profile.json"))
            //{
            //    var settings = p.CreateProfileSettings<PduDefaultProfileSettings>(ps);

            //    var profile = p.CreateDefaultProfile(settings, "simple");

            //    length = 0;
            //    packet = profile.GetPacket(783314087, "łóżko", out length);
            //}


            var ports = SerialPortPlug.AvailablePorts();
            var config = SerialPortConfig.CreateDefault();
            if (ports.Any())
            {
                config.Name = ports.First();
            }

            if (ports.Any())
            {
                using (var modem = new SerialPortPlug(config))
                {
                    if (!modem.IsOpen)
                        return;

                    bool authenticated = false;

                    Task.WaitAny(
                        Task.Run(async () =>
                        {
                            Console.Write("enter pin: ");
                            var pin = Console.ReadLine();

                            var auth = new Authentication();
                            authenticated = await auth.AuthenticateIfNotReady(modem, int.Parse(pin));
                        })
                    );

                    if (!authenticated)
                        return;

                    Task.WaitAny(
                        Task.Run(async () =>
                        {
                            var sms = new PduSms();
                            var send = await sms.Send(modem, 783314087, "łóż ęść");
                        })
                    );

                    //Task.WaitAny(
                    //    Task.Run(async () =>
                    //    {
                    //        bool suc = false;
                    //        var atip = CommandParameter.CreateEmpty(Constants.BasicSuccessfulResponse, true);

                    //        var ati = new SimpleATCommand(ATCommand.DefaultInfo.Command(), atip);
                    //        await ati.ExecuteAsync(modem);
                    //        suc = ati.Succeeded();
                    //    })
                    //);

                    //Task.WaitAny(
                    //    Task.Run(async () =>
                    //    {
                    //        var sca = new ServiceCenter();
                    //        var defined = await sca.IsDefined(modem);
                    //        var intl = await sca.HasInternationalFormat(modem);
                    //        var addr = await sca.GetAddress(modem);
                    //        //var s1 = await sca.SetAddress(modem, 600100200, true);
                    //        //var s2 = await sca.SetAddress(modem, 48601000310, true);
                    //    })
                    //);

                    //Task.WaitAny(
                    //    Task.Run(async () =>
                    //    {
                    //        bool suc = false;
                    //        var mfp = new CommandParameter(Constants.MessageFormat.Pdu.ToValueString(), Constants.BasicSuccessfulResponse, true, false);

                    //        var mf = new ParamATCommand(ATCommand.MessageFormat.Command(), mfp);
                    //        await mf.ExecuteAsync(modem);
                    //        suc = mf.Succeeded();
                    //    })
                    //);

                    //Task.WaitAny(
                    //    Task.Run(async () =>
                    //    {
                    //        bool suc = false;
                    //        var s1 = new CommandParameter($"{length}{Constants.CR}", Constants.ContinueResponse, false, true);
                    //        var s2 = new CommandParameter($"{packet}{Constants.SUB}", Constants.BasicSuccessfulResponse, true, false);

                    //        var ms = new StepwiseATCommand(ATCommand.MessageSend.Command(), new ICommandParameter[] { s1, s2 });
                    //        await ms.ExecuteAsync(modem);
                    //        suc = ms.Succeeded();
                    //    })
                    //);
                }
            }
        }
    }
}
