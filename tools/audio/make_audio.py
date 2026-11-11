"""Compose the music and sound effects -> assets/audio/*.ogg and the Unity project's Resources/Audio.
Run:  python3 tools/audio/make_audio.py   (numpy + ffmpeg)"""
import os, shutil, sys
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import numpy as np
from synth import *

ROOT = os.path.normpath(os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", ".."))
OUT = os.path.join(ROOT, "assets", "audio")
UNITY = os.path.join(ROOT, "unity", "Assets", "Resources", "Audio")

NAMES = ["C", "C#", "D", "Eb", "E", "F", "F#", "G", "Ab", "A", "Bb", "B"]
def midi(n):
    f = freq(n); return int(round(69 + 12 * np.log2(f / 440)))
def name(m): return f"{NAMES[m % 12]}{m // 12 - 1}"
def up(n, semis): return name(midi(n) + semis)

def arp_bar(buf, root, start, eighth, pattern, vel=0.8):
    """pattern: list of (semitones above root, length in eighths)."""
    at = start
    for semis, ln in pattern:
        mix_into(buf, lute(freq(up(root, semis)), ln * eighth + 0.25, vel), at)
        at += ln * eighth

def fade(x, secs=0.4):
    n = min(len(x), int(secs * SR)); x = x.copy(); x[-n:] *= np.linspace(1, 0, n); return x

def finish(x, loop_len=None, rev=0.22):
    y = reverb(x, rev)
    if loop_len: y = loopify(y, loop_len)
    return normalise(y)

# ================================================================== MUSIC
def title_theme():
    bpm = 84; beat = 60 / bpm; bars = 16; L = bars * 3 * beat
    buf = np.zeros(int((L + 3) * SR))
    mel = ("D5:1 E5:.5 F5:.5 G5:1 A5:2 G5:1 F5:1 E5:1 D5:1 E5:3 "
           "F5:1 G5:.5 A5:.5 C6:1 B5:1 A5:1 G5:1 A5:1.5 G5:.5 F5:1 D5:3 "
           "A5:1 A5:.5 B5:.5 C6:1 D6:2 C6:1 B5:1 A5:1 G5:1 A5:3 "
           "F5:1 E5:.5 F5:.5 G5:1 A5:1 G5:1 E5:1 F5:1 E5:1 C5:1 D5:3")
    play(buf, recorder, mel, bpm, 0, 0.9)
    roots = ["D3", "D3", "C3", "A2", "F3", "G3", "F3", "D3", "A2", "D3", "G3", "A2", "F3", "C3", "A2", "D3"]
    for i, r in enumerate(roots):
        arp_bar(buf, r, i * 3 * beat, beat / 2, [(0, 1), (7, 1), (12, 1), (15 if i % 4 != 3 else 16, 1), (12, 1), (7, 1)], 0.55)
        mix_into(buf, tabor(0.4, 0.35), i * 3 * beat)
    mix_into(buf, drone(freq("D3"), L + 1, 0.55, False) + drone(freq("A3"), L + 1, 0.35, False), 0)
    return finish(buf, L, 0.3)

def meadow_theme():
    eighth = 0.2; bar = 6 * eighth
    A = ("D5:2 B4:1 G4:2 A4:1 B4:2 C5:1 D5:3 E5:2 D5:1 C5:2 B4:1 A4:3 D4:3 "
         "G4:2 A4:1 B4:2 C5:1 D5:2 E5:1 F5:3 E5:2 C5:1 D5:2 B4:1 G4:6")
    B = ("G5:2 F5:1 E5:2 D5:1 E5:2 F5:1 G5:3 F5:2 E5:1 D5:2 C5:1 D5:3 A4:3 "
         "B4:2 C5:1 D5:2 E5:1 F5:2 E5:1 D5:2 C5:1 B4:2 A4:1 B4:2 D5:1 G4:6")
    rootsA = ["G2", "G2", "C3", "D3", "G2", "D3", "C3", "G2"]
    rootsB = ["C3", "G2", "F2", "D3", "G2", "F2", "D3", "G2"]
    sections = [(A, rootsA, False), (A, rootsA, True), (B, rootsB, False), (B, rootsB, True)]
    L = 32 * bar
    buf = np.zeros(int((L + 3) * SR))
    bpm8 = 60 / eighth
    for s, (mel, roots, second) in enumerate(sections):
        t0 = s * 8 * bar
        play(buf, recorder, mel, bpm8, t0, 0.95)
        if second:   # second time round a shawm doubles the tune an octave down
            play(buf, shawm, " ".join(f"{up(n.split(':')[0], -12)}:{n.split(':')[1]}" if not n.startswith("r") else n for n in mel.split()), bpm8, t0, 0.35)
        for i, r in enumerate(roots):
            bt = t0 + i * bar
            arp_bar(buf, r, bt, eighth, [(12, 2), (19, 1), (24, 2), (19, 1)], 0.5)
            for at, low in ((0, True), (2, False), (3, True), (5, False)):
                mix_into(buf, tabor(0.3, 0.55 if low else 0.35, low), bt + at * eighth)
                if second and not low: mix_into(buf, jingle(0.2, 0.8), bt + at * eighth)
    dr = drone(freq("G2"), L + 1, 0.5) + drone(freq("D3"), L + 1, 0.3)
    mix_into(buf, dr, 0)
    return finish(buf, L, 0.18)

def forest_theme():
    """World 1-2: a wandering 3/4 tune in A Dorian, lute ostinato under recorder, soft drone."""
    bpm = 100; beat = 60 / bpm; bar = 3 * beat
    A = ("A4:1 C5:1 E5:1 D5:1.5 C5:.5 B4:1 C5:1 A4:1 G4:1 A4:3 "
         "E5:1 G5:1 E5:1 F#5:1.5 E5:.5 D5:1 E5:1 D5:1 C5:1 B4:3")
    B = ("C5:1 D5:1 E5:1 G5:2 E5:1 F#5:1 E5:1 D5:1 E5:3 "
         "D5:1 C5:1 B4:1 C5:1.5 B4:.5 A4:1 G4:1 B4:1 D5:1 A4:3")
    rA = ["A2", "D3", "C3", "A2", "C3", "D3", "E3", "E3"]
    rB = ["C3", "G2", "D3", "A2", "D3", "A2", "G2", "A2"]
    plan = [(A, rA, 0), (A, rA, 1), (B, rB, 1), (A, rA, 0)]
    L = 32 * bar
    buf = np.zeros(int((L + 3) * SR))
    for k, (mel, roots, extra) in enumerate(plan):
        t0 = k * 8 * bar
        play(buf, recorder, mel, bpm, t0, 0.85)
        if extra:   # a second recorder a sixth below, as a consort would
            play(buf, recorder, " ".join(f"{up(n.split(':')[0], -9)}:{n.split(':')[1]}" for n in mel.split()), bpm, t0, 0.3)
        for i, r in enumerate(roots):
            third = 16 if r[0] in "CG" else 15
            arp_bar(buf, r, t0 + i * bar, beat / 2, [(0, 1), (7, 1), (12, 1), (third, 1), (12, 1), (7, 1)], 0.5)
            mix_into(buf, tabor(0.35, 0.3), t0 + i * bar)
            if i % 2 == 1: mix_into(buf, tabor(0.2, 0.18, False), t0 + i * bar + 2 * beat)
    mix_into(buf, drone(freq("A2"), L + 1, 0.45, False) + drone(freq("E3"), L + 1, 0.25, False), 0)
    return finish(buf, L, 0.32)

def crypt_theme():
    """World 1-3: a slow, low tune in E Phrygian; lute ostinato, sparse recorder, a heartbeat on the tabor."""
    bpm = 76; beat = 60 / bpm; bar = 4 * beat
    A = ("E4:2 F4:1 G4:1 A4:3 G4:1 F4:1 E4:1 F4:1 G4:1 E4:4 "
         "B4:2 C5:1 B4:1 A4:2 G4:2 A4:1 G4:1 F4:1 G4:1 E4:4")
    B = ("E5:2 D5:1 C5:1 B4:3 A4:1 C5:1 B4:1 A4:1 G4:1 F4:4 "
         "G4:1 A4:1 B4:1 C5:1 B4:2 A4:2 G4:1 F4:1 G4:1 F4:1 E4:4")
    rA = ["E2", "A2", "F2", "E2", "E2", "A2", "D2", "E2"]
    rB = ["C3", "G2", "A2", "F2", "G2", "A2", "D2", "E2"]
    L = 16 * bar
    buf = np.zeros(int((L + 3) * SR))
    for k, (mel, roots) in enumerate(((A, rA), (B, rB))):
        t0 = k * 8 * bar
        play(buf, recorder, mel, bpm, t0, 0.7)
        for i, r in enumerate(roots):
            third = 16 if r[0] in "CFG" else 15
            arp_bar(buf, r, t0 + i * bar, beat / 2, [(0, 1), (7, 1), (12, 1), (7, 1), (third, 1), (7, 1), (12, 1), (7, 1)], 0.45)
            for b in (0, 0.45):   # heartbeat
                mix_into(buf, tabor(0.3, 0.45 if b == 0 else 0.3), t0 + i * bar + b)
    mix_into(buf, drone(freq("E2"), L + 1, 0.5, False) + drone(freq("B2"), L + 1, 0.25, False), 0)
    return finish(buf, L, 0.5)

def castle_theme():
    """World 1-4: a tense march in D (Aeolian), low shawm over a pounding tabor and drone."""
    bpm = 118; beat = 60 / bpm; bar = 4 * beat
    mel = ("D4:1 D4:.5 F4:.5 A4:1 G4:1 F4:1 E4:1 D4:2 "
           "C4:1 C4:.5 E4:.5 G4:1 F4:1 E4:1 D4:1 E4:2 "
           "D4:1 F4:1 A4:1 C5:1 Bb4:1.5 A4:.5 G4:2 "
           "F4:1 E4:1 D4:1 C4:1 D4:4 ")
    L = 8 * bar * 2
    buf = np.zeros(int((L + 3) * SR))
    for k in range(2):
        t0 = k * 8 * bar
        play(buf, shawm, mel, bpm, t0, 0.7)
        if k: play(buf, recorder, " ".join(f"{up(n.split(':')[0], 12)}:{n.split(':')[1]}" for n in mel.split()), bpm, t0, 0.3)
        for i, r in enumerate(["D2", "D2", "C2", "C2", "D2", "Bb1", "F2", "D2"]):
            for b in range(4):
                mix_into(buf, tabor(0.25, 0.6 if b % 2 == 0 else 0.35, b % 2 == 0), t0 + i * bar + b * beat)
                mix_into(buf, lute(freq(up(r, 12 if b % 2 else 0)), beat, 0.45), t0 + i * bar + b * beat)
    mix_into(buf, drone(freq("D2"), L + 1, 0.5) + drone(freq("A2"), L + 1, 0.3), 0)
    return finish(buf, L, 0.3)

def boss_theme():
    """The Cockatrice: a fast, driving shawm riff and drums."""
    eighth = 0.2 * 0.62; bar = 8 * eighth
    riff = ("D4:1 D4:1 F4:1 D4:1 G4:1 F4:1 E4:1 C4:1 D4:1 D4:1 F4:1 A4:1 G#4:2 A4:2 "
            "Bb4:1 A4:1 G4:1 F4:1 E4:1 F4:1 G4:1 E4:1 D4:2 A3:2 D4:4 ")
    L = 8 * bar
    buf = np.zeros(int((L + 2) * SR))
    for k in range(2):
        play(buf, shawm, riff, 60 / eighth, k * 4 * bar, 0.9)
    play(buf, shawm, " ".join(f"{up(n.split(':')[0], -12)}:{n.split(':')[1]}" for n in (riff * 2).split()), 60 / eighth, 0, 0.4)
    for i in range(8):
        for e in range(8):
            mix_into(buf, tabor(0.18, 0.6 if e % 4 == 0 else 0.35, e % 2 == 0), i * bar + e * eighth)
        mix_into(buf, jingle(0.15, 0.7), i * bar + 4 * eighth)
    mix_into(buf, drone(freq("D2"), L + 1, 0.55), 0)
    return finish(buf, L, 0.15)

def shore_theme():
    """World 2 shore: a rolling 6/8 sea dance in D Mixolydian, recorder and shawm over lute and a swaying drum."""
    eighth = 0.19; bar = 6 * eighth
    A = ("A4:2 D5:1 F#5:2 E5:1 D5:2 C5:1 A4:3 B4:2 C5:1 D5:2 E5:1 F#5:3 E5:3 "
         "A4:2 D5:1 F#5:2 A5:1 G5:2 E5:1 C5:3 D5:2 C5:1 A4:2 B4:1 D5:6")
    B = ("F#5:2 G5:1 A5:2 F#5:1 G5:2 E5:1 C5:3 E5:2 F#5:1 G5:2 E5:1 D5:3 A4:3 "
         "B4:2 C5:1 D5:2 E5:1 F#5:2 E5:1 D5:2 C5:1 A4:2 C5:1 B4:2 G4:1 D5:6")
    rA = ["D3", "D3", "G2", "D3", "D3", "C3", "G2", "D3"]
    rB = ["D3", "C3", "G2", "D3", "G2", "D3", "C3", "D3"]
    L = 32 * bar
    buf = np.zeros(int((L + 3) * SR))
    for k, (mel, roots, second) in enumerate(((A, rA, 0), (A, rA, 1), (B, rB, 0), (B, rB, 1))):
        t0 = k * 8 * bar
        play(buf, recorder, mel, 60 / eighth, t0, 0.9)
        if second: play(buf, shawm, " ".join(f"{up(n.split(':')[0], -12)}:{n.split(':')[1]}" for n in mel.split()), 60 / eighth, t0, 0.3)
        for i, r in enumerate(roots):
            third = 16 if r[0] in "DGC" else 15
            arp_bar(buf, r, t0 + i * bar, eighth, [(0, 2), (7, 1), (12, 2), (third, 1)], 0.5)
            mix_into(buf, tabor(0.35, 0.5), t0 + i * bar)
            mix_into(buf, tabor(0.2, 0.3, False), t0 + i * bar + 3 * eighth)
            if second: mix_into(buf, jingle(0.2, 0.6), t0 + i * bar + 3 * eighth)
    mix_into(buf, drone(freq("D3"), L + 1, 0.45) + drone(freq("A3"), L + 1, 0.25), 0)
    return finish(buf, L, 0.25)

def deep_theme():
    """World 2-2, beneath the waves: a slow, swaying 3/4 waltz in F Lydian, harp-like lute and soft recorder, drowned in reverb."""
    bpm = 96; beat = 60 / bpm; bar = 3 * beat
    A = ("C5:2 F5:1 A5:2 G5:1 E5:3 C5:3 D5:2 E5:1 F5:2 G5:1 A5:3 G5:3 "
         "A5:2 B5:1 C6:2 A5:1 G5:3 E5:3 F5:2 E5:1 D5:2 E5:1 F5:6")
    rA = ["F2", "F2", "C3", "C3", "F2", "G2", "C3", "F2", "F2", "F2", "C3", "C3", "D3", "G2", "C3", "F2"]
    L = 32 * bar
    buf = np.zeros(int((L + 4) * SR))
    for k in range(2):
        t0 = k * 16 * bar
        play(buf, recorder, A, bpm, t0, 0.6)
        if k: play(buf, recorder, " ".join(f"{up(n.split(':')[0], -12)}:{n.split(':')[1]}" for n in A.split()), bpm, t0, 0.25)
        for i, r in enumerate(rA):
            third = 16 if r[0] in "FCG" else 15
            arp_bar(buf, r, t0 + i * bar, beat / 2, [(0, 1), (7, 1), (12, 1), (third, 1), (19, 1), (12, 1)], 0.42)
            mix_into(buf, bell(freq(up(r, 24)), 1.2, 0.15), t0 + i * bar)   # a soft chime, like light on the water
    mix_into(buf, drone(freq("F2"), L + 1, 0.35, False) + drone(freq("C3"), L + 1, 0.2, False), 0)
    return finish(buf, L, 0.65)

def crossing_theme():
    """World 2-3: a brisk 6/8 sea-crossing in A Dorian, shawm lead over lute, drum and drone."""
    eighth = 0.16; bar = 6 * eighth
    A = ("E5:2 A5:1 G5:2 E5:1 D5:2 E5:1 A4:3 C5:2 D5:1 E5:2 G5:1 A5:3 E5:3 "
         "E5:2 A5:1 B5:2 A5:1 G5:2 E5:1 D5:3 E5:2 D5:1 C5:2 B4:1 A4:6")
    B = ("A5:2 B5:1 C6:2 A5:1 G5:2 A5:1 E5:3 D5:2 E5:1 F#5:2 G5:1 A5:3 E5:3 "
         "C5:2 D5:1 E5:2 G5:1 F#5:2 E5:1 D5:3 E5:2 C5:1 B4:2 G4:1 A4:6")
    rA = ["A2", "G2", "A2", "A2", "A2", "G2", "A2", "A2"]
    rB = ["A2", "D3", "C3", "A2", "C3", "D3", "G2", "A2"]
    L = 32 * bar
    buf = np.zeros(int((L + 3) * SR))
    for k, (mel, roots, loud) in enumerate(((A, rA, 0), (A, rA, 1), (B, rB, 1), (A, rA, 1))):
        t0 = k * 8 * bar
        play(buf, shawm if loud else recorder, mel, 60 / eighth, t0, 0.75 if loud else 0.9)
        if loud: play(buf, recorder, mel, 60 / eighth, t0, 0.35)
        for i, r in enumerate(roots):
            third = 16 if r[0] in "CDG" else 15
            arp_bar(buf, r, t0 + i * bar, eighth, [(0, 1), (7, 1), (12, 1), (third, 1), (12, 1), (7, 1)], 0.45)
            for e, lowd in ((0, True), (3, True), (2, False), (5, False)):
                mix_into(buf, tabor(0.22, 0.5 if lowd else 0.3, lowd), t0 + i * bar + e * eighth)
            if loud: mix_into(buf, jingle(0.15, 0.6), t0 + i * bar + 3 * eighth)
    mix_into(buf, drone(freq("A2"), L + 1, 0.45) + drone(freq("E3"), L + 1, 0.25), 0)
    return finish(buf, L, 0.2)

def firmament_theme():
    """World 3-1, ye Firmament: a lilting tune in the manner of early organum - melody doubled a fifth below -
    over a D drone, with bells for the stars."""
    bpm = 88; beat = 60 / bpm; bar = 3 * beat
    A = ("D5:1 E5:1 F5:1 G5:2 F5:1 E5:1 D5:1 C5:1 D5:3 "
         "F5:1 G5:1 A5:1 C6:2 A5:1 G5:1 F5:1 E5:1 D5:3 "
         "A5:1 C6:1 D6:1 C6:2 A5:1 G5:1 A5:1 F5:1 G5:3 "
         "F5:1 E5:1 D5:1 E5:2 C5:1 D5:1 E5:1 C5:1 D5:3")
    L = 2 * 16 * bar
    buf = np.zeros(int((L + 4) * SR))
    for k in range(2):
        t0 = k * 16 * bar
        play(buf, recorder, A, bpm, t0, 0.7)
        play(buf, recorder, " ".join(f"{up(n.split(':')[0], -7)}:{n.split(':')[1]}" for n in A.split()), bpm, t0, 0.35)   # organum at the fifth
        if k: play(buf, shawm, " ".join(f"{up(n.split(':')[0], -12)}:{n.split(':')[1]}" for n in A.split()), bpm, t0, 0.18)
        for i in range(16):
            mix_into(buf, bell(freq(("D6", "A5", "F6", "C6")[i % 4]), 1.4, 0.18), t0 + i * bar)
            mix_into(buf, lute(freq(("D3", "D3", "A2", "D3")[i % 4]), bar, 0.4), t0 + i * bar)
            mix_into(buf, lute(freq(("A3", "A3", "E3", "A3")[i % 4]), bar, 0.3), t0 + i * bar + beat)
    mix_into(buf, drone(freq("D3"), L + 1, 0.45, False) + drone(freq("A3"), L + 1, 0.3, False), 0)
    return finish(buf, L, 0.55)

def waters_theme():
    """World 3-2, ye Waters Above: a slow, flowing 6/8 in G Dorian with organum fifths, harp-like lute and bells."""
    eighth = 0.24; bar = 6 * eighth
    A = ("G4:3 Bb4:2 C5:1 D5:3 F5:3 E5:2 D5:1 C5:2 Bb4:1 A4:6 "
         "Bb4:3 C5:2 D5:1 F5:3 G5:3 F5:2 D5:1 C5:2 A4:1 G4:6")
    roots = ["G2", "Bb2", "C3", "D3", "Bb2", "F2", "C3", "G2"]
    L = 16 * bar
    buf = np.zeros(int((L + 4) * SR))
    for k in range(2):
        t0 = k * 8 * bar
        play(buf, recorder, A, 60 / eighth, t0, 0.65)
        play(buf, recorder, " ".join(f"{up(n.split(':')[0], -7)}:{n.split(':')[1]}" for n in A.split()), 60 / eighth, t0, 0.3)
        for i, r in enumerate(roots):
            third = 16 if r[0] in "BFC" else 15
            arp_bar(buf, r, t0 + i * bar, eighth, [(0, 1), (7, 1), (12, 1), (third, 1), (19, 1), (12, 1)], 0.4)
            mix_into(buf, bell(freq(up(r, 36)), 1.4, 0.13), t0 + i * bar)
    mix_into(buf, drone(freq("G2"), L + 1, 0.35, False) + drone(freq("D3"), L + 1, 0.2, False), 0)
    return finish(buf, L, 0.6)

def eden_theme():
    """World 3-3, ye Naming of ye Beastes: a bright, galloping 2/4 estampie in F Lydian - recorder and shawm,
    lute, a running drum, bells."""
    eighth = 0.15; bar = 4 * eighth
    A = ("F5:1 G5:1 A5:1 C6:1 B5:1 A5:1 G5:2 A5:1 G5:1 F5:1 E5:1 F5:2 C5:2 "
         "D5:1 E5:1 F5:1 A5:1 G5:1 F5:1 E5:2 F5:1 A5:1 G5:1 E5:1 F5:4 ")
    B = ("C6:1 B5:1 A5:1 B5:1 C6:2 A5:2 B5:1 A5:1 G5:1 A5:1 B5:2 G5:2 "
         "A5:1 G5:1 F5:1 G5:1 A5:2 F5:2 G5:1 E5:1 C5:1 E5:1 F5:4 ")
    roots = ["F2", "C3", "F2", "C3", "D3", "C3", "G2", "F2"]
    L = 32 * bar
    buf = np.zeros(int((L + 3) * SR))
    for k, (mel, inst) in enumerate(((A, recorder), (A, shawm), (B, recorder), (B, shawm))):
        t0 = k * 8 * bar
        play(buf, inst, mel, 60 / eighth, t0, 0.8 if inst is recorder else 0.6)
        if inst is shawm: play(buf, recorder, mel, 60 / eighth, t0, 0.35)
        for i, r in enumerate(roots):
            arp_bar(buf, r, t0 + i * bar, eighth, [(0, 1), (12, 1), (7, 1), (16, 1)], 0.45)
            for e in range(4):
                mix_into(buf, tabor(0.18, 0.5 if e % 2 == 0 else 0.3, e % 2 == 0), t0 + i * bar + e * eighth)
            if k % 2: mix_into(buf, jingle(0.15, 0.6), t0 + i * bar + 2 * eighth)
            if i % 2 == 0: mix_into(buf, bell(freq(up(r, 36)), 1.0, 0.12), t0 + i * bar)
    mix_into(buf, drone(freq("F2"), L + 1, 0.4) + drone(freq("C3"), L + 1, 0.25), 0)
    return finish(buf, L, 0.2)

def hunt_theme():
    """World 4-1, ye Boare Hunte: a galloping 6/8 chase in C (Ionian, as the 15th century liked), horn calls over recorder and drum."""
    eighth = 0.15; bar = 6 * eighth
    A = ("G4:2 C5:1 E5:2 G5:1 E5:2 C5:1 G4:3 A4:2 C5:1 F5:2 A5:1 G5:3 E5:3 "
         "G5:2 E5:1 C5:2 E5:1 D5:2 B4:1 G4:3 A4:2 B4:1 C5:2 D5:1 C5:6")
    roots = ["C3", "C3", "F2", "C3", "C3", "G2", "F2", "C3"]
    L = 32 * bar
    buf = np.zeros(int((L + 3) * SR))
    for k in range(4):
        t0 = k * 8 * bar
        play(buf, recorder, A, 60 / eighth, t0, 0.8)
        if k % 2: play(buf, horn, "C4:3 r:3 G4:3 r:3 C5:6 r:6 G4:3 E4:3 C4:6 r:12", 60 / eighth, t0, 0.55)   # the horn calls
        for i, r in enumerate(roots):
            arp_bar(buf, r, t0 + i * bar, eighth, [(0, 2), (7, 1), (12, 2), (16, 1)], 0.45)
            for e, lo in ((0, True), (3, True), (2, False), (5, False)):
                mix_into(buf, tabor(0.2, 0.5 if lo else 0.3, lo), t0 + i * bar + e * eighth)
    mix_into(buf, drone(freq("C3"), L + 1, 0.35) + drone(freq("G3"), L + 1, 0.2), 0)
    return finish(buf, L, 0.22)

def river_theme():
    """World 4-2, ye Otter and ye Swanne: an easy, rippling 3/4 in F, lute running beneath a recorder song."""
    bpm = 100; beat = 60 / bpm; bar = 3 * beat
    A = ("C5:1 F5:1 A5:1 G5:2 F5:1 E5:1 F5:1 G5:1 A5:3 Bb5:1 A5:1 G5:1 F5:2 D5:1 E5:3 C5:3 "
         "D5:1 E5:1 F5:1 A5:2 G5:1 F5:1 E5:1 D5:1 C5:3 D5:1 F5:1 E5:1 D5:2 E5:1 F5:6")
    roots = ["F2", "C3", "F2", "F2", "Bb2", "C3", "C3", "C3", "Bb2", "C3", "F2", "F2", "Bb2", "C3", "F2", "F2"]
    L = 16 * bar * 2
    buf = np.zeros(int((L + 3) * SR))
    for k in range(2):
        t0 = k * 16 * bar
        play(buf, recorder, A, bpm, t0, 0.7)
        if k: play(buf, recorder, " ".join(f"{up(n.split(':')[0], -9)}:{n.split(':')[1]}" for n in A.split()), bpm, t0, 0.3)
        for i, r in enumerate(roots):
            arp_bar(buf, r, t0 + i * bar, beat / 2, [(0, 1), (7, 1), (12, 1), (16, 1), (19, 1), (12, 1)], 0.42)
    mix_into(buf, drone(freq("F2"), L + 1, 0.3, False), 0)
    return finish(buf, L, 0.35)

def heath_theme():
    """World 4-3, ye Falconers' Heath: a wide, airy 4/4 in D Mixolydian - long recorder lines, horn answers, open fifths."""
    bpm = 92; beat = 60 / bpm; bar = 4 * beat
    A = ("D5:2 A5:2 G5:1 F#5:1 E5:2 D5:3 E5:1 F#5:2 A5:2 G5:4 "
         "A5:2 D6:2 C6:1 A5:1 G5:2 F#5:2 E5:1 D5:1 E5:2 C5:2 D5:4")
    roots = ["D3", "D3", "G2", "C3", "D3", "C3", "G2", "D3"]
    L = 16 * bar
    buf = np.zeros(int((L + 3) * SR))
    for k in range(2):
        t0 = k * 8 * bar
        play(buf, recorder, A, bpm, t0, 0.75)
        play(buf, horn, "r:4 A4:2 D5:2 r:8 G4:2 C5:2 r:4 A4:4 D4:4", bpm, t0, 0.35)
        for i, r in enumerate(roots):
            arp_bar(buf, r, t0 + i * bar, beat / 2, [(0, 1), (7, 1), (12, 1), (7, 1), (16, 1), (7, 1), (12, 1), (19, 1)], 0.4)
            mix_into(buf, tabor(0.3, 0.35), t0 + i * bar)
    mix_into(buf, drone(freq("D3"), L + 1, 0.35) + drone(freq("A3"), L + 1, 0.2), 0)
    return finish(buf, L, 0.4)

def unicorn_theme():
    """World 5-1, ye Hunters Set Out: a stately 3/4 basse danse in G Dorian, recorder over lute, bells in the second pass."""
    bpm = 96; beat = 60 / bpm; bar = 3 * beat
    A = ("G4:1 Bb4:1 D5:1 C5:2 Bb4:1 A4:1 G4:1 A4:1 Bb4:3 D5:1 F5:1 E5:1 D5:2 C5:1 Bb4:1 A4:1 Bb4:1 A4:3 "
         "G4:1 Bb4:1 D5:1 G5:2 F5:1 E5:1 D5:1 C5:1 D5:3 C5:1 Bb4:1 A4:1 Bb4:2 A4:1 G4:1 F4:1 A4:1 G4:3")
    roots = ["G2", "C3", "F2", "Bb2", "Bb2", "F2", "C3", "D3", "G2", "Eb3", "C3", "D3", "C3", "Bb2", "F2", "G2"]
    L = 16 * bar * 2
    buf = np.zeros(int((L + 3) * SR))
    for k in range(2):
        t0 = k * 16 * bar
        play(buf, recorder, A, bpm, t0, 0.72)
        for i, r in enumerate(roots):
            arp_bar(buf, r, t0 + i * bar, beat / 2, [(0, 1), (7, 1), (12, 1), (19, 1), (12, 1), (7, 1)], 0.42)
            if k: mix_into(buf, bell(freq(up(r, 24)), 1.2, 0.18), t0 + i * bar)
            mix_into(buf, tabor(0.25, 0.22, False), t0 + i * bar)
    mix_into(buf, drone(freq("G2"), L + 1, 0.3) + drone(freq("D3"), L + 1, 0.18), 0)
    return finish(buf, L, 0.35)

def fountain_theme():
    """World 5-2, ye Fountaine: a gentle, glittering 3/4 in C - recorder song, rippling lute, bells like falling water."""
    bpm = 84; beat = 60 / bpm; bar = 3 * beat
    A = ("E5:2 G5:1 C6:2 B5:1 A5:1 G5:1 E5:1 G5:3 F5:2 A5:1 G5:2 E5:1 D5:1 C5:1 D5:1 E5:3 "
         "E5:2 G5:1 C6:2 D6:1 E6:1 D6:1 C6:1 B5:3 A5:2 F5:1 G5:2 D5:1 E5:1 D5:1 B4:1 C5:3")
    roots = ["C3", "A2", "F2", "C3", "F2", "C3", "G2", "C3", "C3", "A2", "F2", "G2", "F2", "G2", "G2", "C3"]
    L = 16 * bar * 2
    buf = np.zeros(int((L + 3) * SR))
    for k in range(2):
        t0 = k * 16 * bar
        play(buf, recorder, A, bpm, t0, 0.68)
        for i, r in enumerate(roots):
            arp_bar(buf, r, t0 + i * bar, beat / 2, [(0, 1), (7, 1), (12, 1), (16, 1), (19, 1), (24, 1)], 0.38)
            for j, semis in enumerate((31, 28, 24)):   # drops of water
                mix_into(buf, bell(freq(up(r, semis)), 0.9, 0.1), t0 + i * bar + (j + 1.5) * beat * 0.5)
    mix_into(buf, drone(freq("C3"), L + 1, 0.25, False), 0)
    return finish(buf, L, 0.45)

def chase_theme():
    """World 5-3, ye Unicorne at Baye: a breathless 2/4 chase in D minor - shawm over pounding drums, horn calls between phrases."""
    bpm = 150; beat = 60 / bpm; bar = 2 * beat
    A = ("D5:.5 D5:.5 A5:1 G5:.5 F5:.5 E5:1 F5:.5 G5:.5 A5:.5 F5:.5 D5:2 "
         "C5:.5 D5:.5 E5:.5 C5:.5 F5:.5 E5:.5 D5:1 E5:.5 F5:.5 G5:.5 E5:.5 A5:2 "
         "A5:.5 A5:.5 D6:1 C6:.5 Bb5:.5 A5:1 G5:.5 A5:.5 Bb5:.5 G5:.5 A5:2 "
         "F5:.5 G5:.5 A5:.5 F5:.5 E5:.5 F5:.5 G5:.5 E5:.5 D5:.5 E5:.5 C#5:1 D5:2")
    roots = ["D3", "C3", "F2", "D3", "C3", "Bb2", "C3", "A2", "D3", "F2", "G2", "A2", "Bb2", "C3", "A2", "D3"]
    L = 16 * bar * 2
    buf = np.zeros(int((L + 3) * SR))
    for k in range(2):
        t0 = k * 16 * bar
        play(buf, shawm, A, bpm, t0, 0.62)
        play(buf, horn, "r:6 A4:1 D5:1 r:6 E4:1 A4:1 r:6 A4:1 D5:1 r:6 A4:1 D4:1", bpm, t0, 0.4)
        for i, r in enumerate(roots):
            arp_bar(buf, r, t0 + i * bar, beat / 2, [(0, 1), (7, 1), (12, 1), (7, 1)], 0.42)
            for j in range(4):
                mix_into(buf, tabor(0.18, 0.5 if j % 2 == 0 else 0.3, j == 0), t0 + i * bar + j * beat / 2)
    mix_into(buf, drone(freq("D3"), L + 1, 0.3) + drone(freq("A3"), L + 1, 0.15), 0)
    return finish(buf, L, 0.25)

def relic_theme():
    eighth = 0.14; bar = 6 * eighth
    mel = ("G4:1 B4:1 D5:1 G5:1 F5:1 D5:1 E5:1 C5:1 A4:1 F5:1 E5:1 C5:1 "
           "D5:1 B4:1 G4:1 B4:1 D5:1 F5:1 G5:3 D5:3 ") * 2
    L = 8 * bar
    buf = np.zeros(int((L + 2) * SR))
    play(buf, shawm, mel, 60 / eighth, 0, 0.9)
    play(buf, recorder, mel.replace("4:", "5x").replace("5:", "6:").replace("5x", "5:"), 60 / eighth, 0, 0.35)
    for i in range(8):
        for e in range(6):
            mix_into(buf, tabor(0.2, 0.5 if e % 3 == 0 else 0.25, e % 3 == 0), i * bar + e * eighth)
            mix_into(buf, jingle(0.12, 0.6), i * bar + e * eighth)
    mix_into(buf, drone(freq("G2"), L + 1, 0.4), 0)
    return finish(buf, L, 0.12)

def victory():
    buf = np.zeros(int(6 * SR)); bpm = 132
    end = play(buf, shawm, "G4:.5 G4:.5 G4:.5 C5:1.5 r:.25 B4:.25 C5:.5 D5:.5 G5:3", bpm, 0, 1.0)
    play(buf, recorder, "r:2.25 E5:.5 F5:.5 G5:.5 B5:3", bpm, 0, 0.5)
    for i in range(16): mix_into(buf, tabor(0.15, 0.25 + i * 0.02, i % 2 == 0), i * 0.06)
    for n in ("G3", "D4", "G4", "B4"): mix_into(buf, lute(freq(n), 2.5, 0.6), end - 3 * 60 / bpm)
    mix_into(buf, tabor(0.6, 0.8), end - 3 * 60 / bpm)
    return fade(finish(buf, None, 0.3)[: int(5.5 * SR)])

def death():
    buf = np.zeros(int(4 * SR))
    play(buf, recorder, "A4:.5 G4:.5 F4:.5 E4:1 D4:2.5", 110, 0, 0.9)
    mix_into(buf, lute(freq("D3"), 2.5, 0.6), 60 / 110 * 2.5)
    return fade(finish(buf, None, 0.3)[: int(3.6 * SR)])

def game_over():
    buf = np.zeros(int(9 * SR)); bpm = 72
    play(buf, recorder, "D5:1 C5:1 A4:2 Bb4:1 A4:1 G4:2 F4:1 E4:1 D4:4", bpm, 0, 0.85)
    for i, r in enumerate(["D3", "F3", "G3", "C3", "A2", "D3"]):
        mix_into(buf, lute(freq(r), 1.8, 0.5) + lute(freq(up(r, 7)), 1.8, 0.35), i * 2 * 60 / bpm)
    return fade(finish(buf, None, 0.35)[: int(8.5 * SR)])

# ================================================================== SOUND EFFECTS
def sweep(f0, f1, dur, shape="sine", vel=1.0):
    tt = t(dur); n = len(tt)
    f = f0 * (f1 / f0) ** (tt / dur)
    ph = 2 * np.pi * np.cumsum(f) / SR
    y = np.sin(ph) if shape == "sine" else np.sign(np.sin(ph)) * 0.6 + 0.4 * np.sin(2 * ph)
    return y * env(n, 0.005, dur * 0.4) * vel

def add(*xs):
    """Sum signals of different lengths (shorter ones are padded with silence)."""
    n = max(len(x) for x in xs); out = np.zeros(n)
    for x in xs: out[:len(x)] += x
    return out

def fx(x, peak=0.8, rev=0.08):
    return normalise(reverb(x, rev, 0.4), peak)

def sfx():
    S = {}
    S["jump"] = fx(sweep(320, 760, 0.16) * 0.6 + noise(0.16, 2000, 6000, 30, 0.15), 0.6)
    S["stomp"] = fx(add(tabor(0.2, 1.0), sweep(520, 150, 0.12, "sine", 0.5)), 0.75)
    b = np.zeros(int(0.6 * SR)); mix_into(b, bell(freq("B5"), 0.5, 1.0), 0); mix_into(b, bell(freq("E6"), 0.5, 1.0), 0.07)
    S["bezant"] = fx(b, 0.55)
    br = noise(0.4, 300, 4000, 9, 1.0)
    for k in range(6): mix_into(br, noise(0.05, 2000, 8000, 60, 0.6), 0.03 + k * 0.045)
    S["brick"] = fx(br, 0.7)
    S["bump"] = fx(add(tabor(0.16, 1.0), lute(freq("C3"), 0.1, 0.5)), 0.6)
    g = np.zeros(int(0.8 * SR))
    for k, n in enumerate(("G4", "B4", "D5", "G5", "B5", "D6")): mix_into(g, lute(freq(n), 0.5, 0.7, 0.8), k * 0.05)
    S["item_appear"] = fx(g, 0.55, 0.2)
    p = np.zeros(int(1.0 * SR))
    for k, n in enumerate(("C5", "E5", "G5", "C6")): mix_into(p, recorder(freq(n), 0.16, 0.9), k * 0.09)
    mix_into(p, bell(freq("C6"), 0.6, 0.6), 0.36)
    S["powerup"] = fx(p, 0.6, 0.2)
    o = np.zeros(int(1.2 * SR))
    for k, n in enumerate(("E5", "G5", "E6", "C6", "D6", "G6")): mix_into(o, recorder(freq(n), 0.14, 0.9), k * 0.11)
    S["oneup"] = fx(o, 0.6, 0.2)
    S["crossbow"] = fx(add(lute(freq("A2"), 0.18, 1.0, 0.9), noise(0.3, 800, 5000, 14, 0.35)), 0.7)
    tt = t(0.4)
    S["fireball"] = fx(bandpass(rng.standard_normal(len(tt)), 200, 2500) * np.sin(np.pi * tt / 0.4) * 0.9, 0.6)
    S["hurt"] = fx(np.concatenate([shawm(freq("E4"), 0.12, 1.0), shawm(freq("C4"), 0.22, 1.0)]), 0.65)
    # Bayard: clip-clop and a whinny
    h = np.zeros(int(1.0 * SR))
    for at in (0, 0.12, 0.3, 0.42):
        mix_into(h, add(noise(0.05, 900, 3000, 70, 0.9), tabor(0.04, 0.4, False)), at)
    tt = t(0.55); f = 520 + 260 * np.sin(np.pi * tt / 0.55) + 40 * np.sin(2 * np.pi * 22 * tt)
    neigh = bandpass(np.sin(2 * np.pi * np.cumsum(f) / SR) + 0.5 * np.sin(4 * np.pi * np.cumsum(f) / SR), 300, 3000) * env(len(tt), 0.03, 0.2) * 0.5
    mix_into(h, neigh, 0.45)
    S["mount"] = fx(h, 0.65)
    th = np.zeros(int(0.9 * SR)); mix_into(th, neigh * 1.2, 0); mix_into(th, noise(0.3, 400, 3000, 10, 0.4), 0.1)
    S["thrown"] = fx(th, 0.7)
    lo = np.zeros(int(1.4 * SR))
    for k, n in enumerate(("A4", "F4", "D4")): mix_into(lo, recorder(freq(n), 0.38, 0.9), k * 0.36)
    S["bayard_lost"] = fx(lo, 0.55, 0.25)
    S["pause"] = fx(bell(freq("A5"), 0.5, 1.0), 0.45)
    S["kick"] = fx(add(tabor(0.12, 1.0, False), tabor(0.1, 0.6)), 0.7)
    S["soup"] = fx(noise(0.3, 200, 1500, 12, 1.0) * (1 + 0.6 * np.sin(2 * np.pi * 30 * t(0.3))), 0.55)
    S["pole"] = fx(sweep(1200, 300, 0.9) * 0.5 + noise(0.9, 3000, 8000, 3, 0.05), 0.5)
    S["select"] = fx(lute(freq("D4"), 0.3, 1.0, 0.7), 0.5)
    c = np.zeros(int(0.6 * SR)); mix_into(c, lute(freq("D4"), 0.4, 1.0), 0); mix_into(c, lute(freq("A4"), 0.4, 1.0), 0.08)
    S["confirm"] = fx(c, 0.55)
    pu = np.zeros(int(1.6 * SR))
    for k, n in enumerate(("C5", "E5", "G5", "C6", "E6", "G6", "C7")): mix_into(pu, bell(freq(n), 0.9, 0.8), k * 0.08)
    S["purify"] = fx(pu, 0.6, 0.35)
    S["spear"] = fx(noise(0.25, 800, 3000, 12, 0.7) + sweep(900, 400, 0.25, "sine", 0.2), 0.5)
    S["horn"] = fx(np.concatenate([horn(freq("C4"), 0.3, 1.0), horn(freq("G4"), 0.3, 1.0), horn(freq("C5"), 0.6, 1.0)]), 0.6, 0.3)
    S["roar"] = fx(bandpass(rng.standard_normal(int(0.6 * SR)), 80, 900) * env(int(0.6 * SR), 0.05, 0.3) * (1 + 0.5 * np.sin(2 * np.pi * 28 * t(0.6))), 0.6)
    return S

if __name__ == "__main__":
    music = {"title": title_theme, "meadow": meadow_theme, "forest": forest_theme, "crypt": crypt_theme, "castle": castle_theme, "shore": shore_theme, "deep": deep_theme, "crossing": crossing_theme, "firmament": firmament_theme, "waters": waters_theme, "eden": eden_theme, "hunt": hunt_theme, "river": river_theme, "heath": heath_theme, "unicorn": unicorn_theme, "fountain": fountain_theme, "chase": chase_theme, "boss": boss_theme, "relic": relic_theme, "victory": victory, "death": death, "game_over": game_over}
    for n, fn in music.items():
        save_ogg(fn(), os.path.join(OUT, "music", n + ".ogg"))
        print("music", n)
    for n, x in sfx().items():
        save_ogg(x, os.path.join(OUT, "sfx", n + ".ogg"), 6)
    print("sfx done")
    if os.path.isdir(os.path.dirname(UNITY)):
        if os.path.isdir(UNITY): shutil.rmtree(UNITY)
        shutil.copytree(OUT, UNITY)
        print("copied into", UNITY)
