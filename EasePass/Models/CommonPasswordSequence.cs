namespace EasePass.Models;

public class CommonPasswordSequence
{
    private string sequence = "";
    private int minLength = 5;
    private bool entireSequence = false;

    public CommonPasswordSequence(string sequence, int minLength)
    {
        this.sequence = sequence;
        this.minLength = minLength;
        entireSequence = false;
    }

    public CommonPasswordSequence(string sequence)
    {
        this.sequence = sequence;
        entireSequence = true;
    }

    public bool ContainsSequence(string str)
    {
        string lower = str.ToLower();
        if (entireSequence || sequence.Length <= minLength) 
            return lower.Contains(sequence);

        for (int i = 0; i < sequence.Length - minLength; i++)
        {
            if (lower.Contains(sequence.Substring(i, minLength))) 
                return true;
        }
        return false;
    }
}
