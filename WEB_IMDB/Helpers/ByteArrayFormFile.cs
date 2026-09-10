using Microsoft.AspNetCore.Http;

namespace WEB_IMDB.Helpers;

public class ByteArrayFormFile : IFormFile
{
    private readonly byte[] _data;
    private readonly string _name;
    private readonly string _contentType;

    public ByteArrayFormFile(byte[] data, string fileName, string contentType = "image/jpeg")
    {
        _data = data;
        _name = fileName;
        _contentType = contentType;
    }

    public string ContentType => _contentType;
    public string ContentDisposition => $"form-data; name=\"{_name}\"; filename=\"{_name}\"";
    public IHeaderDictionary Headers => new HeaderDictionary();
    public long Length => _data.Length;
    public string Name => _name;
    public string FileName => _name;

    public void CopyTo(Stream target) => target.Write(_data, 0, _data.Length);
    public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
    {
        target.Write(_data, 0, _data.Length);
        return Task.CompletedTask;
    }
    public Stream OpenReadStream()
    {
        var stream = new MemoryStream(_data);
        stream.Position = 0;
        return stream;
    }

    public byte[] GetBytes() => _data;
}
