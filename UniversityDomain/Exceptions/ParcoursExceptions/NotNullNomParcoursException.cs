namespace UniversityDomain.Exceptions.ParcoursExceptions;

[Serializable]
public class NotNullNomParcoursException : Exception
{
    public NotNullNomParcoursException() : base() { }
    public NotNullNomParcoursException(string message) : base(message) { }
    public NotNullNomParcoursException(string message, Exception inner) : base(message, inner) { }
}