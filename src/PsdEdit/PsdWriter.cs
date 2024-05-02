using System.IO;
using System.Text;

namespace PsdEdit;

public sealed class PsdWriter
{
    private IPsdObject _psdObject;

    private FileInfo _target;

    public IPsdObject PsdObject => _psdObject;

    public FileInfo Target => _target;

    private PsdWriter() { }

    private PsdWriter(IPsdObject psdObject) : this() => _psdObject = psdObject;

    private PsdWriter(IPsdObject psdObject, FileInfo target) : this(psdObject) => _target = target;

    public void Save(bool force = false)
    {
        Ensure.ValueNotNull(PsdObject, nameof(PsdObject));

        if (Target is null)
        {
            throw new PsdWriterException(Strings.NoTargetSpecified);
        }


        var builder = this.GetBuilder();
        var mode = force ? FileMode.Create : FileMode.CreateNew;

        try
        {
            using (var fileStream = new FileStream(Target.FullName, mode, FileAccess.ReadWrite))
            using (var writer = new StreamWriter(fileStream))
            {
                writer.Write(builder);
            }
        }
        catch (IOException ioEx) when (File.Exists(Target.FullName))
        {
            throw new PsdWriterException(ioEx, Strings.CannotOverwriteFile);
        }
    }

    public override string ToString() => GetBuilder().ToString();

    private StringBuilder GetBuilder()
    {
        var builder = new StringBuilder();
        var indent = 0;
        this.WriteNextObject(builder, ref indent);
        return builder;
    }

    public static PsdWriter Create() => new PsdWriter();

    public static PsdWriter Create(IPsdObject psdObject) => new PsdWriter(psdObject);

    public static PsdWriter Create(IPsdObject psdObject, FileInfo target) => new PsdWriter(psdObject, target);
}