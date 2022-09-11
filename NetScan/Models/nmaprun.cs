using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetScan.Models
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class nmaprun
    {

        private nmaprunVerbose verboseField;

        private nmaprunDebugging debuggingField;

        private nmaprunHost[] hostField;

        private nmaprunRunstats runstatsField;

        private string scannerField;

        private string argsField;

        private uint startField;

        private string startstrField;

        private decimal versionField;

        private decimal xmloutputversionField;

        /// <remarks/>
        public nmaprunVerbose verbose
        {
            get
            {
                return this.verboseField;
            }
            set
            {
                this.verboseField = value;
            }
        }

        /// <remarks/>
        public nmaprunDebugging debugging
        {
            get
            {
                return this.debuggingField;
            }
            set
            {
                this.debuggingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("host")]
        public nmaprunHost[] host
        {
            get
            {
                return this.hostField;
            }
            set
            {
                this.hostField = value;
            }
        }

        /// <remarks/>
        public nmaprunRunstats runstats
        {
            get
            {
                return this.runstatsField;
            }
            set
            {
                this.runstatsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string scanner
        {
            get
            {
                return this.scannerField;
            }
            set
            {
                this.scannerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string args
        {
            get
            {
                return this.argsField;
            }
            set
            {
                this.argsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint start
        {
            get
            {
                return this.startField;
            }
            set
            {
                this.startField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string startstr
        {
            get
            {
                return this.startstrField;
            }
            set
            {
                this.startstrField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal xmloutputversion
        {
            get
            {
                return this.xmloutputversionField;
            }
            set
            {
                this.xmloutputversionField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nmaprunVerbose
    {

        private byte levelField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte level
        {
            get
            {
                return this.levelField;
            }
            set
            {
                this.levelField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nmaprunDebugging
    {

        private byte levelField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte level
        {
            get
            {
                return this.levelField;
            }
            set
            {
                this.levelField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nmaprunHost
    {

        private nmaprunHostStatus statusField;

        private nmaprunHostAddress[] addressField;

        private nmaprunHostHostnames hostnamesField;

        private nmaprunHostTimes timesField;

        /// <remarks/>
        public nmaprunHostStatus status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("address")]
        public nmaprunHostAddress[] address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        /// <remarks/>
        public nmaprunHostHostnames hostnames
        {
            get
            {
                return this.hostnamesField;
            }
            set
            {
                this.hostnamesField = value;
            }
        }

        /// <remarks/>
        public nmaprunHostTimes times
        {
            get
            {
                return this.timesField;
            }
            set
            {
                this.timesField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nmaprunHostStatus
    {

        private string stateField;

        private string reasonField;

        private byte reason_ttlField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string state
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string reason
        {
            get
            {
                return this.reasonField;
            }
            set
            {
                this.reasonField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte reason_ttl
        {
            get
            {
                return this.reason_ttlField;
            }
            set
            {
                this.reason_ttlField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nmaprunHostAddress
    {

        private string addrField;

        private string addrtypeField;

        private string vendorField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string addr
        {
            get
            {
                return this.addrField;
            }
            set
            {
                this.addrField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string addrtype
        {
            get
            {
                return this.addrtypeField;
            }
            set
            {
                this.addrtypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string vendor
        {
            get
            {
                return this.vendorField;
            }
            set
            {
                this.vendorField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nmaprunHostHostnames
    {

        private nmaprunHostHostnamesHostname hostnameField;

        /// <remarks/>
        public nmaprunHostHostnamesHostname hostname
        {
            get
            {
                return this.hostnameField;
            }
            set
            {
                this.hostnameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nmaprunHostHostnamesHostname
    {

        private string nameField;

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nmaprunHostTimes
    {

        private uint srttField;

        private uint rttvarField;

        private uint toField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint srtt
        {
            get
            {
                return this.srttField;
            }
            set
            {
                this.srttField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint rttvar
        {
            get
            {
                return this.rttvarField;
            }
            set
            {
                this.rttvarField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint to
        {
            get
            {
                return this.toField;
            }
            set
            {
                this.toField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nmaprunRunstats
    {

        private nmaprunRunstatsFinished finishedField;

        private nmaprunRunstatsHosts hostsField;

        /// <remarks/>
        public nmaprunRunstatsFinished finished
        {
            get
            {
                return this.finishedField;
            }
            set
            {
                this.finishedField = value;
            }
        }

        /// <remarks/>
        public nmaprunRunstatsHosts hosts
        {
            get
            {
                return this.hostsField;
            }
            set
            {
                this.hostsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nmaprunRunstatsFinished
    {

        private uint timeField;

        private string timestrField;

        private string summaryField;

        private decimal elapsedField;

        private string exitField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint time
        {
            get
            {
                return this.timeField;
            }
            set
            {
                this.timeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string timestr
        {
            get
            {
                return this.timestrField;
            }
            set
            {
                this.timestrField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string summary
        {
            get
            {
                return this.summaryField;
            }
            set
            {
                this.summaryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal elapsed
        {
            get
            {
                return this.elapsedField;
            }
            set
            {
                this.elapsedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string exit
        {
            get
            {
                return this.exitField;
            }
            set
            {
                this.exitField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class nmaprunRunstatsHosts
    {

        private byte upField;

        private byte downField;

        private ushort totalField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte up
        {
            get
            {
                return this.upField;
            }
            set
            {
                this.upField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte down
        {
            get
            {
                return this.downField;
            }
            set
            {
                this.downField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort total
        {
            get
            {
                return this.totalField;
            }
            set
            {
                this.totalField = value;
            }
        }
    }


}
