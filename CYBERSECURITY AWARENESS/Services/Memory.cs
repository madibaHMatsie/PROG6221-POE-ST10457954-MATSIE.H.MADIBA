


namespace CYBERSECURITY_AWARENESS.Services
{

	public class Memory
{
	private string _userName;
	private string _userInterest;
	private string _lastTopic;

	public void SetUserName(string name)
	{
		_userName = name;
	}

	public string GetUserName()
	{
		return _userName;
	}

	public void SetUserInterest(string interest)
	{
		_userInterest = interest;
	}

	public string GetUserInterest()
	{
		return _userInterest;
	}

	public void SetLastTopic(string topic)
	{
		_lastTopic = topic;
	}

	public string GetLastTopic()
	{
		return _lastTopic;
	}
}
}