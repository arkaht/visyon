using SimpleJSON;

public enum SerializableType
{
	Pattern
}

public interface IJSONSerializable
{
	public JSONNode Serialize();
}