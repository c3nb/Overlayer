namespace Vostok.Commons.Helpers.Windows.WinApi
{
    internal enum JOBOBJECTINFOCLASS
    {
        JobObjectAssociateCompletionPortInformation = 7,
        JobObjectBasicLimitInformation = 2,
        JobObjectBasicUIRestrictions = 4,
        JobObjectCpuRateControlInformation = 15,
        JobObjectEndOfJobTimeInformation = 6,
        JobObjectExtendedLimitInformation = 9,
        JobObjectGroupInformation = 11,
        JobObjectGroupInformationEx = 14,
        JobObjectLimitViolationInformation2 = 35,
        JobObjectNetRateControlInformation = 32,
        JobObjectNotificationLimitInformation = 12,
        JobObjectNotificationLimitInformation2 = 34,
        JobObjectSecurityLimitInformation = 5
    }
}