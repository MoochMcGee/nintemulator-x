using SlimDX.Multimedia;
using SlimDX.XAudio2;
using System;
using System.IO;

namespace Nintemulator.Shared
{
    public class AudioRenderer : IDisposable
    {
        public const int STREAM_COUNT = 4;
        public const int STREAM_MASK = STREAM_COUNT - 1;

        public AudioBuffer buffer;
        public MasteringVoice master;
        public MemoryStream[] streams;
        public SourceVoice source;
        public WaveFormat format;
        public XAudio2 engine;
        public int length;
        public int stream_index;

        public AudioRenderer(IntPtr handle, Params parameters)
        {
            engine = new XAudio2();
            engine.StartEngine();

            format = new WaveFormat();
            format.FormatTag = WaveFormatTag.Pcm;
            format.SamplesPerSecond = parameters.SampleRate;
            format.Channels = parameters.Channels;
            format.BitsPerSample = sizeof(short) * 8;
            format.AverageBytesPerSecond = (format.Channels * sizeof(short) * format.SamplesPerSecond);
            format.BlockAlignment = (short)(format.Channels * sizeof(short));

            length = format.AverageBytesPerSecond / 5; // 200 msec

            master = new MasteringVoice(engine, format.Channels, format.SamplesPerSecond);
            source = new SourceVoice(engine, format);
            source.FrequencyRatio = parameters.Ratio;
            source.Start();

            streams = new MemoryStream[STREAM_COUNT];

            for (int i = 0; i < STREAM_COUNT; i++)
                streams[i] = new MemoryStream(length);

            buffer = new AudioBuffer();
            buffer.AudioBytes = length;
            buffer.AudioData = streams[stream_index++ & STREAM_MASK];
        }

        protected virtual void Dispose(bool disposing)
        {
            source.Stop();
            engine.StopEngine();

            buffer.Dispose();
            buffer = null;

            master.Dispose();
            master = null;

            source.Dispose();
            source = null;

            engine.Dispose();
            engine = null;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
        public void Initialize() { }
        public void Render()
        {
            buffer.AudioData.Seek(0L, SeekOrigin.Begin);

            source.SubmitSourceBuffer(buffer);

            buffer.AudioData = streams[stream_index++ & STREAM_MASK];
            buffer.AudioData.Seek(0L, SeekOrigin.Begin);

            while (source.State.BuffersQueued > 1)
            {
            }
        }
        public void Sample(short sample)
        {
            buffer.AudioData.Write(BitConverter.GetBytes(sample), 0, 2);

            if (buffer.AudioData.Position == length)
                Render();
        }

        public class Params
        {
            public float Ratio;
            public short Channels;
            public int SampleRate;

            public Params(int sampleRate = 48000, short channels = 1, float ratio = 1F)
            {
                this.Ratio = ratio;
                this.Channels = channels;
                this.SampleRate = sampleRate;
            }
        }
    }
}