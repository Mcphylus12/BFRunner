namespace BF;
public class Dumper
{
    private int _indent;

    public void WriteLine(string s)
    {
        for (int i = 0; i < _indent; i++)
        {
            Console.Write("\t");
        }
        Console.WriteLine(s);
    }

    public void Indent() => _indent++;
    public void Unindent() => _indent--;
}
