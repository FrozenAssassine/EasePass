using System;

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

    public bool ContainsSequence(ReadOnlySpan<char> str)
    {

        if (entireSequence || sequence.Length <= minLength)
            return str.Contains(sequence, StringComparison.OrdinalIgnoreCase);

        for (int i = 0; i < sequence.Length - minLength; i++)
        {
            if (str.Contains(sequence.AsSpan(i, minLength), StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}