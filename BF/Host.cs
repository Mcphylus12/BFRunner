

using System.Text;

namespace BF;

internal class Host
{
    private uint _pointer;
    private byte[] _data = new byte[30000];
    private readonly bool _verbose;

    private byte Current { get => _data[_pointer]; set => _data[_pointer] = value; }

    public Host(bool verbose = false)
    {
        Console.InputEncoding = Encoding.ASCII;
        _verbose = verbose;
    }

    public void Run(IOperation operation)
    {
        ProcessOperation(operation);
    }

    private void ProcessOperation(IOperation operation)
    {
        if (operation is Block block) ProcessBlock(block);
        else if (operation is Move move) ProcessMove(move);
        else if (operation is ValueChange change) ProcessChange(change);
        else if (operation is Read read) ProcessRead(read);
        else if (operation is Write write) ProcessWrite(write);
    }

    private void ProcessChange(ValueChange change) => Current = unchecked((byte)(Current + change.Value));

    private void ProcessRead(Read read)
    {
        Console.Write("Enter an input");
        var value = Console.ReadLine();
        Current = (byte)value[0];
    }

    private void ProcessWrite(Write write)
    {
        Console.Write(Encoding.ASCII.GetChars([Current])[0]);
    }

    private void ProcessMove(Move move)
    {
        var newValue = (_pointer + move.Value);
        var normalised = (((newValue % _data.LongLength) + _data.LongLength) % _data.LongLength);

        _pointer = (uint)normalised;
    }

    private void ProcessBlock(Block block)
    {
        if (!block.Loop)
        {
            foreach (var item in block.Contents)
            {
                ProcessOperation(item);
            }

            return;
        }

        while (Current > 0)
        {
            foreach (var item in block.Contents)
            {
                ProcessOperation(item);
            }
        }
    }
}

