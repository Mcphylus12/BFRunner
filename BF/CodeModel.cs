using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF;

internal class CodeModel
{
    private readonly IEnumerator<Token> _enumerator;
    private readonly Block _root;
    public IOperation Root => _root;

    public CodeModel(IList<Token> tokens)
    {
        _enumerator = tokens.GetEnumerator();
        _root = new Block([], false);
        _enumerator.MoveNext();
        Process(_root);
    }

    private void Process(Block block)
    {
        while(_enumerator.Current != Token.Invalid)
        {
            switch (_enumerator.Current)
            {
                case Token.Read: block.Contents.Add(new Read()); _enumerator.MoveNext(); break;
                case Token.Write: block.Contents.Add(new Write()); _enumerator.MoveNext(); break;
                case Token.Increment:
                case Token.Decrement:
                    var valueChange = 0;

                    while (_enumerator.Current == Token.Increment || _enumerator.Current == Token.Decrement)
                    {
                        if (_enumerator.Current == Token.Increment) valueChange++;
                        if (_enumerator.Current == Token.Decrement) valueChange--;
                        _enumerator.MoveNext();
                    }

                    block.Contents.Add(new ValueChange(valueChange));

                    break;

                case Token.LeftPointer:
                case Token.RightPointer:
                    var pointerChange = 0;

                    while (_enumerator.Current == Token.LeftPointer || _enumerator.Current == Token.RightPointer)
                    {
                        if (_enumerator.Current == Token.LeftPointer) pointerChange--;
                        if (_enumerator.Current == Token.RightPointer) pointerChange++;
                        _enumerator.MoveNext();
                    }

                    block.Contents.Add(new Move(pointerChange));

                    break;
                case Token.Start:
                    var innerBlock = new Block([], true);
                    _enumerator.MoveNext();
                    Process(innerBlock);

                    block.Contents.Add(innerBlock);
                    break;
                case Token.End:
                    if (block == _root)
                    {
                        throw new Exception("Mismatched brakcets found a close when we're not in a loop");
                    }
                    _enumerator.MoveNext();
                    return;
            }
        }
    }

    internal void Dump()
    {
        var dumper = new Dumper();
        dumper.WriteLine("Starting Code model dump");
        dumper.WriteLine("-------------------------");
        Root.Dump(dumper);
        dumper.WriteLine("-------------------------");
    }
}

public interface IOperation
{
    void Dump(Dumper dumper);
}

public record struct Move(int Value) : IOperation
{
    public void Dump(Dumper dumper) => dumper.WriteLine($"Move: " + Value);
}

public record struct ValueChange(int Value) : IOperation
{
    public void Dump(Dumper dumper) => dumper.WriteLine($"Change Value: " + Value);
}
public record struct Read() : IOperation
{
    public void Dump(Dumper dumper) => dumper.WriteLine("Read");
}
public record struct Write() : IOperation
{
    public void Dump(Dumper dumper) => dumper.WriteLine("Write");
}

public record Block(IList<IOperation> Contents, bool Loop) : IOperation
{
    public void Dump(Dumper dumper)
    {
        dumper.WriteLine($"Start Loop: {Loop}");
        dumper.Indent();
        foreach (var item in Contents)
        {
            item.Dump(dumper);
        }
        dumper.Unindent();
        dumper.WriteLine("End");
    }
}

