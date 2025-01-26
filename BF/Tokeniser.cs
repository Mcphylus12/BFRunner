namespace BF;

public class Tokeniser
{
    public Tokeniser()
    {
        
    }

    public IList<Token> Process(string source)
    {
        var result = new List<Token>();
        foreach (var character in source)
        {
            Token? token = character switch
            {
                '<' => Token.LeftPointer,
                '>' => Token.RightPointer,
                '+' => Token.Increment,
                '-' => Token.Decrement,
                '.' => Token.Write,
                ',' => Token.Read,
                '[' => Token.Start,
                ']' => Token.End,
                _ => null
            };

            if (token.HasValue)
            {
                result.Add(token.Value);
            }
        }

        return result;
    }
}

public enum Token
{
    Invalid,
    RightPointer,
    LeftPointer,
    Increment,
    Decrement,
    Read,
    Write,
    Start,
    End
}