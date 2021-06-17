using System;

class Email
{
    string message;
    // string receiver;
    // string sender;

    public Email() { }

    public Email(string message)
    {
        this.message = message;
        // this.receiver = ParseReceiver(message);
        // this.sender = ParseSender(message);
    }

    public string GetMessage()
    {
        return message;
    }

    public void SetMessage(string message)
    {
        this.message = message;
    }

    private string ParseSender(string message)
    {
        throw new NotImplementedException();
    }

    private string ParseReceiver(string message)
    {
        throw new NotImplementedException();
    }
}