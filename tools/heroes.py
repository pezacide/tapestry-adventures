"""The four tapestry knights. 20x24 px frames: idle, walk1, walk2, jump."""
from tapestry import parse, flip

W, H = 20, 24

BODY = [  # rows 3..16 (shared by every knight)
    ".....kkSSSSSSkk.....",
    "....kSSSSSSSSSSk....",
    "....kSSSSSSkkkSk....",
    "....kSSSSSSSSSSk....",
    "....kSsSSSSSSsSk....",
    ".....kkssSSSSsskk...",
    "..kSSkTTTTTTTTkSSk..",
    "..kSSkTTTGGTTTkSSk..",
    "..kSSkTGGGGGGTkSSk..",
    "...kkkTTTGGTTTkkk...",
    ".....kTTTGGTTTk.....",
    ".....kGGGGGGGGk.....",
    ".....kTTTTTTTTk.....",
    ".....kTTTTTTTTk.....",
]
LEGS = {
    "idle": [
        ".....kSSSkkSSSk.....",
        ".....kSSSk.kSSSk....",
        ".....kSsSk.kSsSk....",
        ".....kSsSk.kSsSk....",
        "....kkOOOk.kOOOkk...",
        "...kOOOOOk.kOOOOOk..",
        "...kkkkkkk.kkkkkkk..",
    ],
    "walk1": [
        ".....kSSSkkSSSk.....",
        "....kSSSk..kSSk.....",
        "...kSSSk...kSSk.....",
        "...kSsSk...kSsk.....",
        "..kOOOk....kOOk.....",
        ".kOOOOk....kOOOk....",
        ".kkkkkk....kkkkk....",
    ],
    "jump": [
        ".....kSSSkkSSSk.....",
        "....kSSSSk.kSSSSk...",
        "...kOOOOk...kOOOk...",
        "...kkkkkk...kkkk....",
        "....................",
        "....................",
        "....................",
    ],
}
LEGS["walk2"] = [r[::-1] for r in LEGS["walk1"]]

CRESTS = {
    "plume": ["........kPPk........", ".......kPPPPk.......", "......kPPPPPPk......"],
    "antlers": ["....k.k......k.k....", ".....kPk....kPk.....", "......kPk....kPk...."],
    "swan": ["..........kk........", ".........kPPk.......", "........kPPPk......."],
    "sun": [".....k..k..k..k.....", "......k..kPk..k.....", ".......kPPPPPk......"],
}
SHIELD = ["kkkkkk", "kWWWWk", "kWwwWk", "kWwwWk", ".kWWk.", "..kk.."]  # rows 11..16, cols 0..5

KNIGHTS = {
    "rubeus": dict(
        title="Sir Rubeus of the Lion", crest="plume",
        pal=dict(T=(166, 58, 46), G=(224, 176, 64), P=(196, 71, 47), W=(47, 79, 143), w=(236, 196, 90)),
        trait="Balanced - the classic all-rounder"),
    "azura": dict(
        title="Dame Azura of the Swan", crest="swan",
        pal=dict(T=(62, 111, 163), G=(232, 226, 205), P=(241, 236, 222), W=(232, 226, 205), w=(62, 111, 163)),
        trait="Floaty jumper - glides a little at the top of a jump"),
    "verdain": dict(
        title="Sir Verdain of the Stag", crest="antlers",
        pal=dict(T=(79, 122, 58), G=(232, 220, 170), P=(138, 90, 52), W=(52, 80, 42), w=(232, 220, 170)),
        trait="Heavy and strong - can bash bricks while running"),
    "aurelia": dict(
        title="Lady Aurelia of the Sun", crest="sun",
        pal=dict(T=(217, 169, 59), G=(166, 58, 46), P=(242, 201, 76), W=(166, 58, 46), w=(242, 201, 76)),
        trait="Swift runner - fastest on foot, lighter jumps"),
}
BASE = dict(k=(42, 29, 20), S=(184, 192, 200), s=(125, 135, 148), B=(223, 230, 234), O=(107, 74, 47))

def frame(knight, pose):
    rows = [list(r) for r in CRESTS[KNIGHTS[knight]["crest"]] + BODY + LEGS[pose]]
    assert len(rows) == H, len(rows)
    for dy, srow in enumerate(SHIELD):          # shield on the left arm
        for dx, ch in enumerate(srow):
            if ch != ".":
                rows[11 + dy][dx] = ch
    rows[2][18] = "k"                           # sword in the right hand
    for y in range(3, 11):
        rows[y][18], rows[y][19] = "B", "k"
    rows[11][17:20] = list("GGG")
    rows[12][18] = "k"
    rows = ["".join(r) for r in rows]
    for r in rows:
        assert len(r) == W, (knight, pose, len(r), r)
    pal = dict(BASE); pal.update(KNIGHTS[knight]["pal"])
    return parse(rows, pal, W, H)

POSES = ["idle", "walk1", "walk2", "jump"]
