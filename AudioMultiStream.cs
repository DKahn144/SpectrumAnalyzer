using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpectrumAnalyzer
{
    internal class AudioMultiStream : WaveStream, ISampleProvider
    {

        private WaveStream? sourceWaveStream;
        private int sampleRate;
        private int channels;
        private int channelCount;
        private int channelOffset;
        private int channelLength;

        public AudioMultiStream()
        {
        }

        public void OpenFileReader(string filePath)
        {
            sourceWaveStream = new AudioFileReader(filePath);
        }

        public void OpenBufferReader(RawSourceWaveStream? bufferStream, FftBuffer fft = null)
        {
            sourceWaveStream = bufferStream;
        }

        public override WaveFormat WaveFormat => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public int Read(float[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
