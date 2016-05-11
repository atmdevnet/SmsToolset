using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    public class PduProfileManager
    {
        public IPduProfileSettings DefaultPduProfileSettings { get; private set; }
        public bool HasDefaultProfileSettings { get { return DefaultPduProfileSettings != null; } }
        public IPduProfile DefaultProfile { get; private set; }
        public bool HasDefaultProfile { get { return DefaultProfile != null; } }

        private Dictionary<string, IPduProfile> _profiles = new Dictionary<string, IPduProfile>();

        public PduProfileManager()
        {
            this.DefaultPduProfileSettings = loadDefaultProfileSettings("default-send.json", typeof(PduDefaultSendProfileSettings));

            if (this.HasDefaultProfileSettings)
            {
                DefaultProfile = new PduDefaultProfile(this.DefaultPduProfileSettings) { Name = "default-send" };

                _profiles[this.DefaultProfile.Name] = this.DefaultProfile;
            }

            var receiveSettings = loadDefaultProfileSettings("default-receive.json", typeof(PduDefaultReceiveProfileSettings));
            if (receiveSettings != null)
            {
                _profiles["default-receive"] = new PduDefaultProfile(receiveSettings) { Name = "default-receive" };
            }
        }

        public IPduProfileSettings CreateProfileSettings<T>(Stream jsonData) where T : IPduProfileSettings
        {
            Type profileType = typeof(T);

            return loadProfileSettings(jsonData, profileType);
        }

        public IPduProfile CreateDefaultProfile(IPduProfileSettings settings, string name)
        {
            return new PduDefaultProfile(settings) { Name = name };
        }

        public void AddProfile(IPduProfile profile)
        {
            _profiles[profile.Name] = profile;
        }

        public IPduProfile GetProfile(string name)
        {
            return _profiles[name];
        }

        public bool RemoveProfile(string name)
        {
            return _profiles.Remove(name);
        }

        public IPduProfile this[string name] { get { return _profiles[name]; } set { _profiles[value.Name] = value; } }

        public Dictionary<string, IPduProfile>.Enumerator GetEnumerator()
        {
            return _profiles.GetEnumerator();
        }

        public bool ContainsProfile(string name)
        {
            return _profiles.ContainsKey(name);
        }

        public IEnumerable<string> GetProfileNames()
        {
            return _profiles.Keys.AsEnumerable();
        }

        public int Count
        {
            get { return _profiles.Count; }
        }

        private IPduProfileSettings loadDefaultProfileSettings(string name, Type profileType)
        {
            IPduProfileSettings result = null;

            using (var profileResource = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(PduProfileManager), name))
            {
                var profile = loadProfileSettings(profileResource, profileType);
                if (profile != null)
                {
                    result = profile;
                }
            }

            return result;
        }

        private IPduProfileSettings loadProfileSettings(Stream jsonFile, Type profileType)
        {
            IPduProfileSettings result = null;

            try
            {
                var reader = new StreamReader(jsonFile);

                var profileText = reader.ReadToEnd();
                var profileBytes = Encoding.UTF8.GetBytes(profileText);

                using (var profileStream = new MemoryStream(profileBytes))
                {
                    var deserializer = new DataContractJsonSerializer(profileType);
                    var profileSettings = deserializer.ReadObject(profileStream);

                    if (profileSettings is IPduProfileSettings)
                    {
                        result = profileSettings as IPduProfileSettings;
                    }
                }
            }
            catch { }

            return result;
        }
    }


    

}
