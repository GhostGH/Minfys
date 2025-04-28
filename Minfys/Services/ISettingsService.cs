using Minfys.Models;

namespace Minfys.Services;

public interface ISettingsService
{
    AudioOptions Options { get; }
    void Save(AudioOptions options);
    string ExtractDefaultAudio();
}