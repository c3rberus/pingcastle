﻿//
// Copyright (c) Ping Castle. All rights reserved.
// https://www.pingcastle.com
//
// Licensed under the Non-Profit OSL. See LICENSE file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;
using System.Xml.Serialization;
using PingCastle.Data;
using PingCastle.Rules;

namespace PingCastle.Healthcheck
{

	public interface IGPOReference
	{
		string GPOName { get; set; }
		string GPOId { get; set; }
	}

    [DebuggerDisplay("{Name}")]
    public class HealthCheckGroupMemberData
    {
        public string Name { get; set; }

        public bool IsExternal { get; set; }

        public string DistinguishedName { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsActive { get; set; }

        public bool IsLocked { get; set; }

        public bool DoesPwdNeverExpires { get; set; }

        public bool CanBeDelegated { get; set; }

        public DateTime LastLogonTimestamp { get; set; }

        public DateTime PwdLastSet { get; set; }

		public bool SmartCardRequired { get; set; }

		public bool IsService { get; set; }
    }

    [DebuggerDisplay("{GroupName}")]
    public class HealthCheckGroupData
    {
        public HealthCheckGroupData()
        {
            Level = PingCastleReportDataExportLevel.Full;
        }

		[IgnoreDataMember]
		[XmlIgnore]
        public PingCastleReportDataExportLevel Level { get; set; }

        public string GroupName { get; set; }

        public int NumberOfMember { get; set; }

        public int NumberOfMemberDisabled { get; set; }

        public int NumberOfMemberPwdNotRequired { get; set; }

        public int NumberOfMemberPwdNeverExpires { get; set; }

        public int NumberOfMemberLocked { get; set; }

        public int NumberOfMemberInactive { get; set; }

        public int NumberOfMemberActive { get; set; }

        public int NumberOfMemberEnabled { get; set; }

        public int NumberOfMemberCanBeDelegated { get; set; }

        public int NumberOfExternalMember { get; set; }

		public int NumberOfSmartCardRequired { get; set; }

		public int NumberOfServiceAccount { get; set; }

        public bool ShouldSerializeMembers() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<HealthCheckGroupMemberData> Members { get; set; }

    }

	[DebuggerDisplay("FQDN: {DnsName} SiD: {Sid} NetBIOS: {NetbiosName} Forest: FQDN: {ForestName} SID: {ForestSid} NetBIOS {ForestNetbios}")]
    public class HealthCheckTrustDomainInfoData
    {
        public string DnsName { get; set; }
        public string NetbiosName { get; set; }
        public string Sid { get; set; }
        public DateTime CreationDate { get; set; }
        public string ForestName { get; set; }
        public string ForestSid { get; set; }
        public string ForestNetbios { get; set; }

        private DomainKey _domain;
		[IgnoreDataMember]
        [XmlIgnore]
        public DomainKey Domain
        {
            get
            {
                if (_domain == null)
                {
                    _domain = DomainKey.Create(DnsName, Sid, NetbiosName);
                }
                return _domain;
            }
			set
			{
				_domain = value;
			}
        }

		private bool _forestSet = false;
        private DomainKey _forest;
		[IgnoreDataMember]
		[XmlIgnore]
        public DomainKey Forest
        {
            get
            {
				if (!_forestSet)
                {
					_forestSet = true;
					if (String.Equals(DnsName, ForestName, StringComparison.InvariantCultureIgnoreCase))
						_forest = Domain;
					else
					{
						_forest = DomainKey.Create(ForestName, ForestSid, ForestNetbios);
					}
                }
                return _forest;
            }
			set
			{
				_forest = value;
			}
        }

    }

    [DebuggerDisplay("{TrustPartner} {CreationDate}")]
    public class HealthCheckTrustData
    {
        ///<summary>
        ///TrustPartner is garanteed to be in lowercase.
        ///</summary>
        public string TrustPartner { get; set; }

        public int TrustAttributes { get; set; }

        public int TrustDirection { get; set; }

        public int TrustType { get; set; }

        public DateTime CreationDate { get; set; }

        public bool IsActive { get; set; }

        public string SID { get; set; }

        public string NetBiosName { get; set; }

        public List<HealthCheckTrustDomainInfoData> KnownDomains { get; set; }

        private DomainKey _domain;
		[IgnoreDataMember]
		[XmlIgnore]
        public DomainKey Domain
        {
            get
            {
                if (_domain == null)
                {
                    _domain = DomainKey.Create(TrustPartner, SID, NetBiosName);
                }
                return _domain;
            }
        }

    }

	[DebuggerDisplay("{GPOName} {UserName}")]
	public class GPOInfo : IGPOReference
	{
		public string GPOName { get; set; }
		public string GPOId { get; set; }
		public bool IsDisabled { get; set; }
		public List<string> AppliedTo { get; set; }
	}

    [DebuggerDisplay("{GPOName} {UserName}")]
	public class GPPPassword : IGPOReference
    {
        public string UserName { get; set; }
        public string Other { get; set; }
        public string Password { get; set; }
        public DateTime Changed { get; set; }


        public string Type { get; set; }

        public string GPOName { get; set; }
		public string GPOId { get; set; }
    }

	[DebuggerDisplay("{GPOName} {FileName}")]
	public class GPPFileDeployed : IGPOReference
	{
		public string FileName { get; set; }
		public string Type { get; set; }
		public string GPOName { get; set; }
		public string GPOId { get; set; }
		public List<HealthcheckScriptDelegationData> Delegation { get; set; }
	}

	[DebuggerDisplay("{Property} {Value}")]
    public class GPPSecurityPolicyProperty
    {
        public GPPSecurityPolicyProperty()
        {
        }
        public GPPSecurityPolicyProperty(string property, int value)
        {
            this.Property = property;
            this.Value = value;
        }
        public string Property { get; set; }
        public int Value { get; set; }
    }

    [DebuggerDisplay("{GPOName}")]
	public class GPPSecurityPolicy : IGPOReference
    {
        public List<GPPSecurityPolicyProperty> Properties { get; set; }

        public string GPOName { get; set; }
		public string GPOId { get; set; }
    }

    [DebuggerDisplay("{GPOName} {User} {Privilege}")]
	public class GPPRightAssignment : IGPOReference
    {
        public string User { get; set; }

        public string Privilege { get; set; }

        public string GPOName { get; set; }
		public string GPOId { get; set; }
    }

    [DebuggerDisplay("{GPOName} {User} {Privilege}")]
	public class GPOMembership : IGPOReference
    {
        public string GPOName { get; set; }
		public string GPOId { get; set; }
        public string User { get; set; }
        public string MemberOf { get; set; }
    }

	[DebuggerDisplay("{GPOName} {Order} {Server}")]
	public class GPOEventForwardingInfo : IGPOReference
	{
		public string GPOName { get; set; }
		public string GPOId { get; set; }
		public int Order { get; set; }
		public string Server { get; set; }
	}

    [DebuggerDisplay("{Name}")]
    public class HealthcheckAccountDetailData
    {
        public string DistinguishedName { get; set; }

        public string Name { get; set; }

        public DateTime LastLogonDate { get; set; }

        public DateTime CreationDate { get; set; }
    }

    public class HealthcheckAccountData
    {
        public HealthcheckAccountData()
        {
            Level = PingCastleReportDataExportLevel.Full;
        }

        public void Add(HealthcheckAccountData x)
        {
            Number += x.Number;
            NumberActive += x.NumberActive;
            NumberBadPrimaryGroup += x.NumberBadPrimaryGroup;
            NumberDesEnabled += x.NumberDesEnabled;
            NumberDisabled += x.NumberDisabled;
            NumberEnabled += x.NumberEnabled;
            NumberInactive += x.NumberInactive;
            NumberLocked += x.NumberLocked;
            NumberPwdNeverExpires += x.NumberPwdNeverExpires;
            NumberPwdNotRequired += x.NumberPwdNotRequired;
            NumberReversibleEncryption += x.NumberReversibleEncryption;
            NumberSidHistory += x.NumberSidHistory;
            NumberTrustedToAuthenticateForDelegation += x.NumberTrustedToAuthenticateForDelegation;
            NumberDuplicate += x.NumberDuplicate;
            NumberNoPreAuth += x.NumberNoPreAuth;
        }

		public void SetProxy(HealthcheckAccountData proxy)
		{
			if (proxy.ListBadPrimaryGroup == null)
				proxy.ListBadPrimaryGroup = new List<HealthcheckAccountDetailData>();
			ListBadPrimaryGroup = proxy.ListBadPrimaryGroup;
			if (proxy.ListDesEnabled == null)
				proxy.ListDesEnabled = new List<HealthcheckAccountDetailData>();
			ListDesEnabled = proxy.ListDesEnabled;
			if (proxy.ListDomainSidHistory == null)
				proxy.ListDomainSidHistory = new List<HealthcheckSIDHistoryData>();
			ListDomainSidHistory = proxy.ListDomainSidHistory;
			if (proxy.ListDuplicate == null)
				proxy.ListDuplicate = new List<HealthcheckAccountDetailData>();
			ListDuplicate = proxy.ListDuplicate;
			if (proxy.ListInactive == null)
				proxy.ListInactive = new List<HealthcheckAccountDetailData>();
			ListInactive = proxy.ListInactive;
			if (proxy.ListLocked == null)
				proxy.ListLocked = new List<HealthcheckAccountDetailData>();
			ListLocked = proxy.ListLocked;
			if (proxy.ListNoPreAuth == null)
				proxy.ListNoPreAuth = new List<HealthcheckAccountDetailData>();
			ListNoPreAuth = proxy.ListNoPreAuth;
			if (proxy.ListPwdNeverExpires == null)
				proxy.ListPwdNeverExpires = new List<HealthcheckAccountDetailData>();
			ListPwdNeverExpires = proxy.ListPwdNeverExpires;
			if (proxy.ListPwdNotRequired == null)
				proxy.ListPwdNotRequired = new List<HealthcheckAccountDetailData>();
			ListPwdNotRequired = proxy.ListPwdNotRequired;
			if (proxy.ListReversibleEncryption == null)
				proxy.ListReversibleEncryption = new List<HealthcheckAccountDetailData>();
			ListReversibleEncryption = proxy.ListReversibleEncryption;
			if (proxy.ListSidHistory == null)
				proxy.ListSidHistory = new List<HealthcheckAccountDetailData>();
			ListSidHistory = proxy.ListSidHistory;
			if (proxy.ListTrustedToAuthenticateForDelegation == null)
				proxy.ListTrustedToAuthenticateForDelegation = new List<HealthcheckAccountDetailData>();
			ListTrustedToAuthenticateForDelegation = proxy.ListTrustedToAuthenticateForDelegation;
		}

		public void ClearProxy()
		{
			ListBadPrimaryGroup = null;
			ListDesEnabled = null;
			ListDomainSidHistory = null;
			ListDuplicate = null;
			ListInactive = null;
			ListLocked = null;
			ListNoPreAuth = null;
			ListPwdNeverExpires = null;
			ListPwdNotRequired = null;
			ListReversibleEncryption = null;
			ListSidHistory = null;
			ListTrustedToAuthenticateForDelegation = null;
		}

		[IgnoreDataMember]
		[XmlIgnore]
        public PingCastleReportDataExportLevel Level { get; set; }

        public int Number { get; set; }

        public int NumberEnabled { get; set; }

        public int NumberDisabled { get; set; }

        public int NumberActive { get; set; }

        public int NumberInactive { get; set; }

        public bool ShouldSerializeListInactive() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<HealthcheckAccountDetailData> ListInactive { get; set; }

        public int NumberLocked { get; set; }

        public bool ShouldSerializeListLocked() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<HealthcheckAccountDetailData> ListLocked { get; set; }

        public int NumberPwdNeverExpires { get; set; }

        public bool ShouldSerializeListPwdNeverExpires() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<HealthcheckAccountDetailData> ListPwdNeverExpires { get; set; }

        public int NumberSidHistory { get; set; }

        public bool ShouldSerializeListSidHistory() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<HealthcheckAccountDetailData> ListSidHistory { get; set; }

        public bool ShouldSerializeListDomainSidHistory() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
        public List<HealthcheckSIDHistoryData> ListDomainSidHistory { get; set; }

        public int NumberPwdNotRequired { get; set; }

        public bool ShouldSerializeListPwdNotRequired() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<HealthcheckAccountDetailData> ListPwdNotRequired { get; set; }

        public int NumberBadPrimaryGroup { get; set; }

        public bool ShouldSerializeListBadPrimaryGroup() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<HealthcheckAccountDetailData> ListBadPrimaryGroup { get; set; }

        public int NumberDesEnabled { get; set; }

        public bool ShouldSerializeListDesEnabled() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<HealthcheckAccountDetailData> ListDesEnabled { get; set; }

        public int NumberTrustedToAuthenticateForDelegation { get; set; }

        public bool ShouldSerializeListTrustedToAuthenticateForDelegation() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<HealthcheckAccountDetailData> ListTrustedToAuthenticateForDelegation { get; set; }

        public int NumberReversibleEncryption { get; set; }
        
        public bool ShouldSerializeListReversibleEncryption() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<HealthcheckAccountDetailData> ListReversibleEncryption { get; set; }

        public int NumberDuplicate { get; set; }
        
        public bool ShouldSerializeListDuplicate() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<HealthcheckAccountDetailData> ListDuplicate { get; set; }

        public int NumberNoPreAuth { get; set; }
        public bool ShouldSerializeListNoPreAuth() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<HealthcheckAccountDetailData> ListNoPreAuth { get; set; }

    }

    public class HealthcheckRiskRule : IRuleScore
	{
        public HealthcheckRiskRule()
        {
            Level = PingCastleReportDataExportLevel.Full;
        }

		[IgnoreDataMember]
		[XmlIgnore]
        public PingCastleReportDataExportLevel Level { get; set; }

        public int Points { get; set; }

		// we are using a xml serialization trick to be resilient if a new RiskRuleCategory is added in the future
		[XmlIgnore]
		public RiskRuleCategory Category { get; set; }

		[XmlElement("Category")]
		public string CategoryAsString
		{
			get
			{
				return Category.ToString();
			}
			set
			{
				try
				{
					Category = (RiskRuleCategory)Enum.Parse(typeof(RiskRuleCategory), value);
				}
				catch
				{
					Category = RiskRuleCategory.Unknown;
				}
			}
		}

		// we are using a xml serialization trick to be resilient if a new RiskModelCategory is added in the future
		[XmlIgnore]
		public RiskModelCategory Model { get; set; }

		[XmlElement("Model")]
		public string ModelAsString
		{
			get
			{
				return Model.ToString();
			}
			set
			{
				try
				{
					Model = (RiskModelCategory)Enum.Parse(typeof(RiskModelCategory), value);
				}
				catch
				{
					Model = RiskModelCategory.Unknown;
				}
			}
		}

		public string RiskId { get; set; }

        public string Rationale { get; set; }

		public bool ShouldSerializeDetails() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<string> Details { get; set; }
    }

    [DebuggerDisplay("{OperatingSystem}")]
    public class HealthcheckOSData
    {
        public HealthcheckOSData()
        {
        }
        public HealthcheckOSData(string OS)
        {
            this.OperatingSystem = OS;
        }
        public string OperatingSystem { get; set; }
        public int NumberOfOccurence { get; set; }
		public HealthcheckAccountData data { get; set; }
    }

    [DebuggerDisplay("{LoginScript}")]
    public class HealthcheckLoginScriptData
    {
        public HealthcheckLoginScriptData()
        {
        }
        public HealthcheckLoginScriptData(string script, int numOccurence)
        {
            this.LoginScript = script;
            this.NumberOfOccurence = numOccurence;
        }
        public string LoginScript { get; set; }
        public int NumberOfOccurence { get; set; }

		public List<HealthcheckScriptDelegationData> Delegation { get; set; }
    }

    [DebuggerDisplay("{GPOName} {Action} {CommandLine}")]
	public class HealthcheckGPOLoginScriptData : IGPOReference
    {

        public string GPOName { get; set; }

		public string GPOId { get; set; }

        public string Action { get; set; }

        public string Source { get; set; }

        public string CommandLine { get; set; }

        public string Parameters { get; set; }

		public List<HealthcheckScriptDelegationData> Delegation { get; set; }
    }

    [DebuggerDisplay("{DistinguishedName} {Account} {Right}")]
    public class HealthcheckDelegationData
    {
        public string DistinguishedName { get; set; }

        public string Account { get; set; }

		public string SecurityIdentifier { get; set; }

        public string Right { get; set; }
    }

	[DebuggerDisplay("{Account} {Right}")]
	public class HealthcheckScriptDelegationData
	{
		public string Account { get; set; }

		public string Right { get; set; }
	}

	[DebuggerDisplay("{GPOName} {Account} {Right}")]
	public class GPODelegationData : IGPOReference
	{
		public string GPOName { get; set; }

		public string GPOId { get; set; }

		public string Item { get; set; }

		public string Account { get; set; }

		public string Right { get; set; }
	}

    [DebuggerDisplay("{GPOName} {Category} {Value}")]
    public class GPOAuditSimpleData : IGPOReference
    {
        public string GPOName { get; set; }

        public string GPOId { get; set; }

        public string Category { get; set; }

        public int Value { get; set; }
    }

    [DebuggerDisplay("{GPOName} {SubCategory} {Value}")]
    public class GPOAuditAdvancedData : IGPOReference
    {
        public string GPOName { get; set; }

        public string GPOId { get; set; }

        public Guid SubCategory { get; set; }

        public int Value { get; set; }
    }
    //public class HealthcheckSiteTopologyData
    //{

    //    public string SiteName { get; set; }
    //    public List<string> Subnets { get; set; }
    //}

    public class HealthcheckCertificateData
    {
        public string Source { get; set; }
        public string Store { get; set; }
        public byte[] Certificate { get; set; }
    }

    [DebuggerDisplay("{DomainSid} {FriendlyName}")]
    public class HealthcheckSIDHistoryData
    {
        public string DomainSid { get; set; }
        public string FriendlyName { get; set; }
		public string NetBIOSName { get; set; }
        public DateTime FirstDate { get; set; }
        public DateTime LastDate { get; set; }
        public int Count { get; set; }

        private DomainKey _domain;
		[IgnoreDataMember]
		[XmlIgnore]
        public DomainKey Domain
        {
            get
            {
                if (_domain == null)
                {
					_domain = DomainKey.Create(FriendlyName, DomainSid, NetBIOSName);
                }
                return _domain;
            }
        }
    }

    [Flags]
    public enum SMBSecurityModeEnum
    {
        NotTested = 0,
        None = 1,
        SmbSigningEnabled = 2,
        SmbSigningRequired = 4,
    }

    [DebuggerDisplay("{DCName}")]
    public class HealthcheckDomainController
    {
        public string DCName { get; set; }

		public DateTime CreationDate { get; set; }

		public DateTime StartupTime { get; set; }
		
		public DateTime LastComputerLogonDate { get; set; }

        public string DistinguishedName { get; set; }

		public string OperatingSystem { get; set; }

        public string OwnerSID { get; set; }

        public string OwnerName { get; set; }

        public bool HasNullSession { get; set; }

		public bool SupportSMB1 { get; set; }

        public SMBSecurityModeEnum SMB1SecurityMode { get; set; }

		public bool SupportSMB2OrSMB3 { get; set; }

		public SMBSecurityModeEnum SMB2SecurityMode { get; set; }

		public bool RemoteSpoolerDetected { get; set; }

        public List<string> IP { get; set; }

		public List<string> FSMO { get; set; }

		public List<string> LDAPSProtocols { get; set; }
	}

	[DebuggerDisplay("{SiteName}")]
    public class HealthcheckSite
    {
        public string SiteName { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public List<string> Networks { get; set; }
    }

    [DebuggerDisplay("{Domain}")]
    public class HealthcheckData : IRiskEvaluation, IPingCastleReport
    {
        public string EngineVersion { get; set; }
        public DateTime GenerationDate { get; set; }

		public string GetHumanReadableFileName()
		{
			return "ad_hc_" + DomainFQDN + ".html";
		}

		public string GetMachineReadableFileName()
		{
			return "ad_hc_" + DomainFQDN + ".xml";
		}

		public void SetExportLevel(PingCastleReportDataExportLevel level)
		{
			Level = level;
		}

        // this property is used to limit the serialization of some properties
        private PingCastleReportDataExportLevel _level;
        public PingCastleReportDataExportLevel Level
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
                // if the value is changed, propagate it to properties which needs it
                if (UserAccountData != null)
                {
                    UserAccountData.Level = value;
                }
                if (ComputerAccountData != null)
                {
                    ComputerAccountData.Level = value;
                }
                if (PrivilegedGroups != null)
                {
                    foreach (var group in PrivilegedGroups)
                    {
                        group.Level = value;
                    }
                }
                if (RiskRules != null)
                {
                    foreach (var rule in RiskRules)
                    {
                        rule.Level = value;
                    }
                }
				if (ControlPaths != null && ControlPaths.Data != null)
				{
					foreach (var d in ControlPaths.Data)
					{
						d.Level = value;
					}
				}
            }
        }

		public void InitializeReportingData()
		{
			version = new Version(EngineVersion.Split(' ')[0]);

			applicableRules = new List<RuleBase<HealthcheckData>>();
			foreach (var rule in RuleSet<HealthcheckData>.Rules)
			{
				object[] models = rule.GetType().GetCustomAttributes(typeof(RuleIntroducedInAttribute), true);
				if (models != null && models.Length != 0)
				{
					RuleIntroducedInAttribute model = (RuleIntroducedInAttribute)models[0];
					if (model.Version <= version)
					{
						applicableRules.Add(rule);
					}
				}
				else
				{
					applicableRules.Add(rule);
				}
			}
		}
		[IgnoreDataMember]
		[XmlIgnore]
		public Version version { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public List<RuleBase<HealthcheckData>> applicableRules {get;set;}

		private Dictionary<string, GPOInfo> _GPOInfoDic;
		[IgnoreDataMember]
		[XmlIgnore]
		public Dictionary<string, GPOInfo> GPOInfoDic
		{
			get
			{
				if (_GPOInfoDic != null) 
					return _GPOInfoDic;
				_GPOInfoDic = new Dictionary<string, GPOInfo>(StringComparer.OrdinalIgnoreCase);
				foreach (var gpo in GPOInfo)
				{
					if (!_GPOInfoDic.ContainsKey(gpo.GPOId))
					{
						_GPOInfoDic.Add(gpo.GPOId, gpo);
					}
				}
				return _GPOInfoDic;
			}
		}

        public string DomainFQDN { get; set; }
        public string NetBIOSName { get; set; }
        public string ForestFQDN { get; set; }

        public DateTime DomainCreation { get; set; }
        public string DomainSid { get; set; }

        public int DomainFunctionalLevel { get; set; }
        public int ForestFunctionalLevel { get; set; }
		public int SchemaVersion { get; set; }
		public int SchemaInternalVersion { get; set; }
		public bool IsRecycleBinEnabled { get; set; }

		public DateTime SchemaLastChanged { get; set; }
        public int NumberOfDC { get; set; }
        public int GlobalScore { get; set; }
        public int StaleObjectsScore { get; set; }
        public int PrivilegiedGroupScore { get; set; }
        public int TrustScore { get; set; }
        public int AnomalyScore { get; set; }

        public bool ShouldSerializeTrusts() { return (int)Level <= (int)PingCastleReportDataExportLevel.Light; }
        public List<HealthCheckTrustData> Trusts { get; set; }

        public bool ShouldSerializeReachableDomains() { return (int)Level <= (int)PingCastleReportDataExportLevel.Light; }
        public List<HealthCheckTrustDomainInfoData> ReachableDomains { get; set; }

		public bool ShouldSerializeDomainControllers() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
        public List<HealthcheckDomainController> DomainControllers { get; set; }

        public bool ShouldSerializeSites() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
        public List<HealthcheckSite> Sites { get; set; }

        public bool ShouldSerializePreWindows2000AnonymousAccess() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
        public bool PreWindows2000AnonymousAccess { get; set; }
        public bool ShouldSerializeDsHeuristicsAnonymousAccess() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
        public bool DsHeuristicsAnonymousAccess { get; set; }

		public bool ShouldSerializeDsHeuristicsAdminSDExMaskModified() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
		public bool DsHeuristicsAdminSDExMaskModified { get; set; }

		public bool ShouldSerializeDsHeuristicsDoListObject() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
		public bool DsHeuristicsDoListObject { get; set; }

        public bool ShouldSerializeRiskRules() { return (int)Level <= (int)PingCastleReportDataExportLevel.Light; }
        public List<HealthcheckRiskRule> RiskRules { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public IList<IRuleScore> AllRiskRules { get { return RiskRules.ConvertAll(x => { return (IRuleScore)x; }); } }

        public bool ShouldSerializeUserAccountData() { return (int)Level <= (int)PingCastleReportDataExportLevel.Light; }
        public HealthcheckAccountData UserAccountData { get; set; }

        public bool ShouldSerializeComputerAccountData() { return (int)Level <= (int)PingCastleReportDataExportLevel.Light; }
        public HealthcheckAccountData ComputerAccountData { get; set; }

        public bool ShouldSerializeOperatingSystem() { return (int)Level <= (int)PingCastleReportDataExportLevel.Light; }
        public List<HealthcheckOSData> OperatingSystem { get; set; }
		// DO NOT USE - former data
        public bool ShouldSerializeOperatingSystemDC() { return (int)Level <= (int)PingCastleReportDataExportLevel.Light; }
        public List<HealthcheckOSData> OperatingSystemDC { get; set; }

		public bool ShouldSerializeGPOInfo() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
		public List<GPOInfo> GPOInfo { get; set; }

        public bool ShouldSerializeLoginScript() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
        public List<HealthcheckLoginScriptData> LoginScript { get; set; }

		public bool ShouldSerializeLastADBackup() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
		public DateTime LastADBackup { get; set; }

		public bool ShouldSerializeLAPSInstalled() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
		public DateTime LAPSInstalled { get; set; }

        public bool ShouldSerializeKrbtgtLastChangeDate() { return (int)Level <= (int)PingCastleReportDataExportLevel.Light; }
        public DateTime KrbtgtLastChangeDate { get; set; }

		public bool ShouldSerializeKrbtgtLastVersion() { return (int)Level <= (int)PingCastleReportDataExportLevel.Light; }
		public int KrbtgtLastVersion { get; set; }

		public bool ShouldSerializeExchangePrivEscVulnerable() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
		public bool ExchangePrivEscVulnerable { get; set; }

        public bool ShouldSerializeAdminLastLoginDate() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
        public DateTime AdminLastLoginDate { get; set; }

        public bool ShouldSerializeAdminAccountName() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public string AdminAccountName { get; set; }

        public bool ShouldSerializeGPPPassword() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
        public List<GPPPassword> GPPPassword { get; set; }

		public bool ShouldSerializeGPPFileDeployed() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
		public List<GPPFileDeployed> GPPFileDeployed { get; set; }

        public bool ShouldSerializeGPPRightAssignment() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<GPPRightAssignment> GPPRightAssignment { get; set; }

		public bool ShouldSerializeGPPLoginAllowedOrDeny() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
		public List<GPPRightAssignment> GPPLoginAllowedOrDeny { get; set; }

        public bool ShouldSerializeGPOAuditSimple() { return (int)Level <= (int)PingCastleReportDataExportLevel.Light; }
        public List<GPOAuditSimpleData> GPOAuditSimple { get; set; }

        public bool ShouldSerializeGPOAuditAdvanced() { return (int)Level <= (int)PingCastleReportDataExportLevel.Light; }
        public List<GPOAuditAdvancedData> GPOAuditAdvanced { get; set; }

        public bool ShouldSerializeGPPPasswordPolicy() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
        public List<GPPSecurityPolicy> GPPPasswordPolicy { get; set; }

        public bool ShouldSerializeGPOLsaPolicy() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
        public List<GPPSecurityPolicy> GPOLsaPolicy { get; set; }

        public bool ShouldSerializeGPOScreenSaverPolicy() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
        public List<GPPSecurityPolicy> GPOScreenSaverPolicy { get; set; }

		public bool ShouldSerializeGPOEventForwarding() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
		public List<GPOEventForwardingInfo> GPOEventForwarding { get; set; }

		public bool ShouldSerializeGPODelegation() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
		public List<GPODelegationData> GPODelegation { get; set; }

        public bool ShouldSerializeGPOLocalMembership() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<GPOMembership> GPOLocalMembership { get; set; }

        public bool ShouldSerializeTrustedCertificates() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
        public List<HealthcheckCertificateData> TrustedCertificates { get; set; }

        public bool ShouldSerializePrivilegedGroups() { return (int)Level <= (int)PingCastleReportDataExportLevel.Light; }
        public List<HealthCheckGroupData> PrivilegedGroups { get; set; }

        public bool ShouldSerializeAllPrivilegedMembers() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<HealthCheckGroupMemberData> AllPrivilegedMembers { get; set; }

        public bool ShouldSerializeAdminSDHolderNotOKCount() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
        public int AdminSDHolderNotOKCount { get; set; }

        public bool ShouldSerializeAdminSDHolderNotOK() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<HealthcheckAccountDetailData> AdminSDHolderNotOK { get; set; }

        public bool ShouldSerializeSmartCardNotOKCount() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
        public int SmartCardNotOKCount { get; set; }

        public bool ShouldSerializeSmartCardNotOK() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<HealthcheckAccountDetailData> SmartCardNotOK { get; set; }

        public bool ShouldSerializeDelegations() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<HealthcheckDelegationData> Delegations { get; set; }

        public bool ShouldSerializeGPOLoginScript() { return (int)Level <= (int)PingCastleReportDataExportLevel.Full; }
        public List<HealthcheckGPOLoginScriptData> GPOLoginScript { get; set; }

        //public bool ShouldSerializeSiteTopology() { return (int)Level <= (int)HealthcheckDataLevel.Normal; }
        //public List<HealthcheckSiteTopologyData> SiteTopology { get; set; }

        public bool ShouldSerializeDomainControllerWithNullSessionCount() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
        public int DomainControllerWithNullSessionCount { get; set; }

        public bool ShouldSerializeSIDHistoryAuditingGroupPresent() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
        public bool SIDHistoryAuditingGroupPresent { get; set; }

        public bool ShouldSerializeMachineAccountQuota() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
        public int MachineAccountQuota { get; set; }

		public bool ShouldSerializeListHoneyPot() { return (int)Level <= (int)PingCastleReportDataExportLevel.Normal; }
		public List<HealthcheckAccountDetailData> ListHoneyPot { get; set; }

        private DomainKey _domain;
		[IgnoreDataMember]
		[XmlIgnore]
        public DomainKey Domain
        {
            get
            {
                if (_domain == null)
                {
                    _domain = DomainKey.Create(DomainFQDN, DomainSid, NetBIOSName);
                }
                return _domain;
            }
        }

        private DomainKey _forest;
		[IgnoreDataMember]
		[XmlIgnore]
        public DomainKey Forest
        {
            get
            {
                if (_forest == null)
                {
					if (String.Equals(ForestFQDN, DomainFQDN, StringComparison.InvariantCultureIgnoreCase))
                    {
                        _forest = Domain;
                    }
                    else
                    {
                        string sid = null;
						string netbiosname = null;
                        if (Trusts != null)
                        {
                            foreach (var trust in Trusts)
                            {
								if (String.Equals(trust.TrustPartner, ForestFQDN, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    if (!String.IsNullOrEmpty(trust.SID))
                                    {
                                        sid = trust.SID;
										netbiosname = trust.NetBiosName;
                                    }
                                    break;
                                }
                            }
                        }
						_forest = DomainKey.Create(ForestFQDN, sid, netbiosname);
                    }
                }
                return _forest;
            }
        }

		[IgnoreDataMember]
		[XmlIgnore]
		public IList<DomainKey> DomainKnown
		{
			get
			{
				var output = new List<DomainKey>();
				output.Add(Domain);
				if (Domain.DomainName != Forest.DomainName)
					output.Add(Forest);
				if (Trusts != null)
				{
					foreach (var t in Trusts)
					{
						output.Add(t.Domain);
						if (t.KnownDomains != null)
						{
							foreach (var d in t.KnownDomains)
							{
								output.Add(d.Domain);
								if (d.Forest != null)
									output.Add(d.Forest);
							}
						}
					}
				}
				if (ReachableDomains != null)
				{
					foreach (var d in ReachableDomains)
					{
						output.Add(d.Domain);
						if (d.Forest != null)
							output.Add(d.Forest);
					}
				}
				return output;
			}
		}

		public CompromiseGraphData ControlPaths { get; set; }
    }
}
