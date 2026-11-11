"""A tiny medieval band in numpy: recorder, lute, hurdy-gurdy drone, shawm, bells and tabor.
Everything is synthesised here, so the music and sounds are original and free to use."""
import math, os, subprocess, tempfile, wave
import numpy as np

SR = 44100
rng = np.random.default_rng(7)

NOTE = {"C": -9, "D": -7, "E": -5, "F": -4, "G": -2, "A": 0, "B": 2}
def freq(name):
    """'D4', 'F#5', 'Bb3' -> Hz."""
    n, rest = name[0], name[1:]
    acc = 0
    while rest and rest[0] in "#b":
        acc += 1 if rest[0] == "#" else -1; rest = rest[1:]
    octave = int(rest)
    return 440.0 * 2 ** ((NOTE[n] + acc + (octave - 4) * 12) / 12)

def t(dur): return np.arange(int(dur * SR)) / SR

def env(n, a=0.01, r=0.05, sustain=1.0):
    e = np.full(n, sustain, dtype=np.float64)
    na, nr = max(1, int(a * SR)), max(1, int(r * SR))
    na, nr = min(na, n), min(nr, n)
    e[:na] = np.linspace(0, sustain, na)
    e[n - nr:] *= np.linspace(1, 0, nr)
    return e

def lowpass(x, k=0.2):
    """One-pole low-pass (vectorised with an IIR via cumulative trick is awkward; use FFT)."""
    X = np.fft.rfft(x); f = np.fft.rfftfreq(len(x), 1 / SR)
    cutoff = k * SR / 2
    X *= 1 / np.sqrt(1 + (f / max(cutoff, 1)) ** 4)
    return np.fft.irfft(X, len(x))

def bandpass(x, lo, hi):
    X = np.fft.rfft(x); f = np.fft.rfftfreq(len(x), 1 / SR)
    X[(f < lo) | (f > hi)] *= 0.05
    return np.fft.irfft(X, len(x))

# ------------------------------------------------------------------ instruments
def recorder(f, dur, vel=1.0):
    tt = t(dur); n = len(tt)
    vib = 1 + 0.006 * np.sin(2 * np.pi * 5.2 * tt) * np.clip((tt - 0.12) * 4, 0, 1)
    ph = 2 * np.pi * np.cumsum(f * vib) / SR
    tone = np.sin(ph) + 0.18 * np.sin(2 * ph) + 0.06 * np.sin(3 * ph)
    breath = bandpass(rng.standard_normal(n), f * 0.8, f * 3) * 0.04
    chiff = np.exp(-tt * 60) * bandpass(rng.standard_normal(n), 1500, 6000) * 0.15
    return (tone + breath + chiff) * env(n, 0.025, min(0.08, dur * 0.4), 0.9) * vel * 0.32

def shawm(f, dur, vel=1.0):
    tt = t(dur); n = len(tt)
    ph = 2 * np.pi * f * tt + 0.01 * np.sin(2 * np.pi * 6 * tt)
    tone = sum((1 / k) * np.sin(k * ph) for k in (1, 3, 5, 7, 9)) + 0.4 * np.sin(2 * ph) + 0.2 * np.sin(4 * ph)
    tone = bandpass(tone, 300, 3500)
    return np.tanh(tone * 1.4) * env(n, 0.02, min(0.07, dur * 0.4), 0.85) * vel * 0.26

def lute(f, dur, vel=1.0, bright=0.5):
    """Karplus-Strong plucked string, computed a period at a time."""
    n = int(dur * SR); N = max(2, int(SR / f))
    buf = rng.uniform(-1, 1, N)
    buf = bright * buf + (1 - bright) * np.convolve(buf, np.ones(3) / 3, "same")
    out = np.empty(n + N)
    out[:N] = buf
    decay = 0.996
    i = N
    while i < n + N:
        prev = out[i - N:i]
        nxt = decay * 0.5 * (prev + np.roll(prev, -1))
        m = min(N, n + N - i)
        out[i:i + m] = nxt[:m]
        i += m
    y = out[:n]
    return y * env(n, 0.002, min(0.05, dur * 0.3)) * vel * 0.38

def drone(f, dur, vel=1.0, buzz=True):
    """Hurdy-gurdy: a bowed sawtooth drone, with the 'trompette' buzz pulsing on the beat."""
    tt = t(dur); n = len(tt)
    ph = 2 * np.pi * f * tt
    tone = sum(((-1) ** (k + 1)) / k * np.sin(k * ph) for k in range(1, 12))
    tone = lowpass(tone, 0.12)
    out = tone * 0.18
    if buzz:
        out *= 1 + 0.25 * np.maximum(0, np.sin(2 * np.pi * 2.0 * tt)) ** 6
    return out * env(n, 0.15, 0.3, 1.0) * vel

def horn(f, dur, vel=1.0):
    """A hunting horn: a brassy tone with a soft swell."""
    tt = t(dur); n = len(tt)
    ph = 2 * np.pi * f * tt
    tone = sum((0.9 ** k) * np.sin(k * ph) for k in range(1, 9))
    tone = lowpass(tone, 0.08 + 0.04 * min(1.0, dur))
    return np.tanh(tone * 0.9) * env(n, 0.06, min(0.12, dur * 0.4), 0.9) * vel * 0.3

def organ(f, dur, vel=1.0):
    """A portative organ: flue pipes - fundamental, octave and twelfth, a little wind in the tone, steady while held."""
    tt = t(dur); n = len(tt)
    ph = 2 * np.pi * f * tt
    y = np.sin(ph) + 0.5 * np.sin(2 * ph) + 0.3 * np.sin(3 * ph) + 0.15 * np.sin(4 * ph + 0.3)
    y += 0.02 * rng.standard_normal(n)
    return y * env(n, 0.03, min(0.08, dur * 0.3), 1.0) * vel * 0.22

def bell(f, dur, vel=1.0):
    tt = t(dur); n = len(tt)
    parts = ((1, 1.0, 3.0), (2.76, 0.5, 5.0), (5.4, 0.25, 8.0), (8.93, 0.12, 12.0))
    y = sum(a * np.sin(2 * np.pi * f * r * tt) * np.exp(-tt * d) for r, a, d in parts)
    return y * env(n, 0.001, 0.02) * vel * 0.3

def tabor(dur=0.25, vel=1.0, low=True):
    tt = t(dur); n = len(tt)
    if low:
        f = 95 * np.exp(-tt * 6) + 55
        y = np.sin(2 * np.pi * np.cumsum(f) / SR) * np.exp(-tt * 14)
        y += bandpass(rng.standard_normal(n), 150, 1200) * np.exp(-tt * 40) * 0.4
    else:   # the 'tak' on the rim, with a snare buzz
        y = bandpass(rng.standard_normal(n), 1200, 7000) * np.exp(-tt * 35) * 0.7
        y += np.sin(2 * np.pi * 330 * tt) * np.exp(-tt * 50) * 0.3
    return y * vel * 0.5

def jingle(dur=0.2, vel=1.0):
    tt = t(dur); n = len(tt)
    y = bandpass(rng.standard_normal(n), 5000, 12000) * (np.exp(-tt * 18)) * (1 + 0.5 * np.sin(2 * np.pi * 40 * tt))
    return y * vel * 0.18

def noise(dur, lo, hi, decay=10.0, vel=1.0):
    tt = t(dur); n = len(tt)
    return bandpass(rng.standard_normal(n), lo, hi) * np.exp(-tt * decay) * vel

# ------------------------------------------------------------------ mixing
def mix_into(buf, sig, at):
    i = int(at * SR)
    if i >= len(buf): return
    m = min(len(sig), len(buf) - i)
    buf[i:i + m] += sig[:m]

def reverb(x, amount=0.25, length=1.2):
    n = int(length * SR)
    ir = rng.standard_normal(n) * np.exp(-np.arange(n) / SR * 4.5)
    ir = lowpass(ir, 0.35); ir /= np.sqrt(np.sum(ir ** 2))
    L = len(x) + n
    size = 1 << (L - 1).bit_length()
    wet = np.fft.irfft(np.fft.rfft(x, size) * np.fft.rfft(ir, size), size)[:L]
    out = np.zeros(L); out[:len(x)] += x
    return out + wet * amount

def loopify(x, loop_len):
    """Fold the tail past loop_len back onto the start so the loop is seamless."""
    n = int(loop_len * SR)
    out = x[:n].copy()
    tail = x[n:]
    out[:len(tail)] += tail[:n]
    return out

def normalise(x, peak=0.89):
    m = np.max(np.abs(x)) or 1.0
    return x * (peak / m)

def save_ogg(x, path, quality=5):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    pcm = (np.clip(x, -1, 1) * 32767).astype(np.int16)
    with tempfile.NamedTemporaryFile(suffix=".wav", delete=False) as f:
        tmp = f.name
    with wave.open(tmp, "wb") as w:
        w.setnchannels(1); w.setsampwidth(2); w.setframerate(SR); w.writeframes(pcm.tobytes())
    subprocess.run(["ffmpeg", "-y", "-loglevel", "error", "-i", tmp, "-c:a", "libvorbis", "-q:a", str(quality), path], check=True)
    os.remove(tmp)

# ------------------------------------------------------------------ score helper
def play(buf, inst, tune, bpm, start=0.0, vel=1.0, legato=0.95, **kw):
    """tune: 'D4:1 E4:.5 r:.5 ...' (beats). Returns end time."""
    beat = 60.0 / bpm
    at = start
    for tok in tune.split():
        name, _, b = tok.partition(":")
        d = float(b) * beat
        if name != "r":
            for nm in name.split("+"):   # chords: D3+A3
                mix_into(buf, inst(freq(nm), d * legato + 0.04, vel, **kw) if kw else inst(freq(nm), d * legato + 0.04, vel), at)
        at += d
    return at
