namespace AIIncidentAnalysisAuthServiceAPI.Exceptions;

public class CacheException(string message, Exception innerException) : Exception(message, innerException);