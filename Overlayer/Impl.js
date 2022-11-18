/**
 * @returns {string} HitMargin in Strict Difficulty
 */
function SHit();
/**
 * @returns {number} TooEarly in Strict Difficulty
 */
function STE();
/**
 * @returns {number} VeryEarly in Strict Difficulty
 */
function SVE();
/**
 * @returns {number} EarlyPerfect in Strict Difficulty
 */
function SEP();
/**
 * @returns {number} Perfect in Strict Difficulty
 */
function SP();
/**
 * @returns {number} LatePerfect in Strict Difficulty
 */
function SLP();
/**
 * @returns {number} VeryLate in Strict Difficulty
 */
function SVL();
/**
 * @returns {number} TooLate in Strict Difficulty
 */
function STL();
/**
 * @returns {string} HitMargin in Normal Difficulty
 */
function NHit();
/**
 * @returns {number} TooEarly in Normal Difficulty
 */
function NTE();
/**
 * @returns {number} VeryEarly in Normal Difficulty
 */
function NVE();
/**
 * @returns {number} EarlyPerfect in Normal Difficulty
 */
function NEP();
/**
 * @returns {number} Perfect in Normal Difficulty
 */
function NP();
/**
 * @returns {number} LatePerfect in Normal Difficulty
 */
function NLP();
/**
 * @returns {number} VeryLate in Normal Difficulty
 */
function NVL();
/**
 * @returns {number} TooLate in Normal Difficulty
 */
function NTL();
/**
 * @returns {string} HitMargin in Lenient Difficulty
 */
function LHit();
/**
 * @returns {number} TooEarly in Lenient Difficulty
 */
function LTE();
/**
 * @returns {number} VeryEarly in Lenient Difficulty
 */
function LVE();
/**
 * @returns {number} EarlyPerfect in Lenient Difficulty
 */
function LEP();
/**
 * @returns {number} Perfect in Lenient Difficulty
 */
function LP();
/**
 * @returns {number} LatePerfect in Lenient Difficulty
 */
function LLP();
/**
 * @returns {number} VeryLate in Lenient Difficulty
 */
function LVL();
/**
 * @returns {number} TooLate in Lenient Difficulty
 */
function LTL();
/**
 * @returns {string} HitMargin in Current Difficulty
 */
function CurHit();
/**
 * @returns {number} TooEarly in Current Difficulty
 */
function CurTE();
/**
 * @returns {number} VeryEarly in Current Difficulty
 */
function CurVE();
/**
 * @returns {number} EarlyPerfect in Current Difficulty
 */
function CurEP();
/**
 * @returns {number} Perfect in Current Difficulty
 */
function CurP();
/**
 * @returns {number} LatePerfect in Current Difficulty
 */
function CurLP();
/**
 * @returns {number} VeryLate in Current Difficulty
 */
function CurVL();
/**
 * @returns {number} TooLate in Current Difficulty
 */
function CurTL();
/**
 * @returns {string} Current Difficulty
 */
function CurDifficulty();
/**
 * @param {number} opt
 * @returns {string} KeyCode:Judgement
 */
function KeyJudge(opt);
/**
 * @returns {number} Current KPS
 */
function CurKps();
/**
 * @param {number} opt
 * @returns {number} Current Planet's Radius
 */
function Radius(opt);
/**
 * @param {number} opt
 * @returns {number} Current Pitch
 */
function Pitch(opt);
/**
 * @param {number} opt
 * @returns {number} Pitch In Editor
 */
function EditorPitch(opt);
/**
 * @returns {string} TooEarly Judgement Hex Code
 */
function TEHex();
/**
 * @returns {string} VeryEarly Judgement Hex Code
 */
function VEHex();
/**
 * @returns {string} EarlyPerfect Judgement Hex Code
 */
function EPHex();
/**
 * @returns {string} Perfect Judgement Hex Code
 */
function PHex();
/**
 * @returns {string} LatePerfect Judgement Hex Code
 */
function LPHex();
/**
 * @returns {string} VeryLate Judgement Hex Code
 */
function VLHex();
/**
 * @returns {string} TooLate Judgement Hex Code
 */
function TLHex();
/**
 * @returns {string} Multipress Judgement Hex Code
 */
function MPHex();
/**
 * @returns {string} FailMiss Judgement Hex Code
 */
function FMHex();
/**
 * @returns {string} FailOverload Judgement Hex Code
 */
function FOHex();
/**
 * @returns {string} Title
 */
function Title();
/**
 * @returns {string} Author
 */
function Author();
/**
 * @returns {string} Artist
 */
function Artist();
/**
 * @returns {number} Start Tile
 */
function StartTile();
/**
 * @param {number} opt
 * @returns {number} Difficulty Of Current Level
 */
function Difficulty(opt);
/**
 * @param {number} opt
 * @returns {number} Accuracy
 */
function Accuracy(opt);
/**
 * @param {number} opt
 * @returns {number} Progress
 */
function Progress(opt);
/**
 * @returns {number} Check Point Used Count
 */
function CheckPoint();
/**
 * @returns {number} Current Check Point Index
 */
function CurCheckPoint();
/**
 * @returns {number} Total Check Points Count
 */
function TotalCheckPoint();
/**
 * @param {number} opt
 * @returns {number} XAccuracy
 */
function XAccuracy(opt);
/**
 * @returns {number} Fail Count
 */
function FailCount();
/**
 * @returns {number} Miss Count
 */
function MissCount();
/**
 * @returns {number} Overload Count
 */
function Overloads();
/**
 * @param {number} opt
 * @returns {number} Perceived Bpm
 */
function CurBpm(opt);
/**
 * @param {number} opt
 * @returns {number} Tile Bpm
 */
function TileBpm(opt);
/**
 * @param {number} opt
 * @returns {number} Perceived KPS
 */
function RecKps(opt);
/**
 * @param {number} opt
 * @returns {number} Best Progress
 */
function BestProgress(opt);
/**
 * @returns {number} Least Check Point Used Count
 */
function LeastCheckPoint();
/**
 * @param {number} opt
 * @returns {number} Start Progress
 */
function StartProgress(opt);
/**
 * @returns {number} Now Minute Of Music
 */
function CurMinute();
/**
 * @returns {number} Now Second Of Music
 */
function CurSecond();
/**
 * @returns {number} Now MilliSecond Of Music
 */
function CurMilliSecond();
/**
 * @returns {number} Total Minute Of Music
 */
function TotalMinute();
/**
 * @returns {number} Total Second Of Music
 */
function TotalSecond();
/**
 * @returns {number} Total MilliSecond Of Music
 */
function TotalMilliSecond();
/**
 * @returns {number} Left Tile Count
 */
function LeftTile();
/**
 * @returns {number} Total Tile Count
 */
function TotalTile();
/**
 * @returns {number} Current Tile Count
 */
function CurTile();
/**
 * @returns {number} Current Level Try Count
 */
function Attempts();
/**
 * @returns {number} Year Of System Time
 */
function Year();
/**
 * @returns {number} Month Of System Time
 */
function Month();
/**
 * @returns {number} Day Of System Time
 */
function Day();
/**
 * @returns {number} Hour Of System Time
 */
function Hour();
/**
 * @returns {number} Minute Of System Time
 */
function Minute();
/**
 * @returns {number} Second Of System Time
 */
function Second();
/**
 * @returns {number} MilliSecond Of System Time
 */
function MilliSecond();
/**
 * @returns {number} Multipress Count
 */
function Multipress();
/**
 * @returns {number} Combo
 */
function Combo();
/**
 * @param {number} opt
 * @returns {number} Frame Rate
 */
function Fps(opt);
/**
 * @param {number} opt
 * @returns {number} FrameTime
 */
function FrameTime(opt);
/**
 * @returns {number} CPU Core's Count
 */
function ProcessorCount();
/**
 * @param {number} opt
 * @returns {number} Total Physics Memory Size (GigaBytes)
 */
function MemoryGBytes(opt);
/**
 * @param {number} opt
 * @returns {number} Adofai's Cpu Usage (Percentage)
 */
function CpuUsage(opt);
/**
 * @param {number} opt
 * @returns {number} Total Cpu Usage (Percentage)
 */
function TotalCpuUsage(opt);
/**
 * @param {number} opt
 * @returns {number} Adofai's Memory Usage (Percentage)
 */
function MemoryUsage(opt);
/**
 * @param {number} opt
 * @returns {number} Total Memory Usage (Percentage)
 */
function TotalMemoryUsage(opt);
/**
 * @param {number} opt
 * @returns {number} Adofai's Memory Usage (GigaBytes)
 */
function MemoryUsageGBytes(opt);
/**
 * @param {number} opt
 * @returns {number} Total Memory Usage (GigaBytes)
 */
function TotalMemoryUsageGBytes(opt);
/**
 * @param {number} opt
 * @returns {number} PlayPoint(PP) In Adofai.gg
 */
function PlayPoint(opt);
/**
 * @param {string} opt
 * @returns {number} Death Count For Progress
 */
function ProgressDeath(opt);
/**
 * @returns {number} Score in Current Difficulty
 */
function Score();
/**
 * @returns {number} Score in Lenient Difficulty
 */
function LScore();
/**
 * @returns {number} Score in Normal Difficulty
 */
function NScore();
/**
 * @returns {number} Score in Strict Difficulty
 */
function SScore();
/**
 * @param {number} opt
 * @returns {number} Hit Timing
 */
function Timing(opt);
/**
 * @param {number} opt
 * @returns {number} Average Hit Timing
 */
function TimingAvg(opt);
const KeyCode =
{
    None: 0,
    Backspace: 8,
    Delete: 0x7F,
    Tab: 9,
    Clear: 12,
    Return: 13,
    Pause: 19,
    Escape: 27,
    Space: 0x20,
    Keypad0: 0x100,
    Keypad1: 257,
    Keypad2: 258,
    Keypad3: 259,
    Keypad4: 260,
    Keypad5: 261,
    Keypad6: 262,
    Keypad7: 263,
    Keypad8: 264,
    Keypad9: 265,
    KeypadPeriod: 266,
    KeypadDivide: 267,
    KeypadMultiply: 268,
    KeypadMinus: 269,
    KeypadPlus: 270,
    KeypadEnter: 271,
    KeypadEquals: 272,
    UpArrow: 273,
    DownArrow: 274,
    RightArrow: 275,
    LeftArrow: 276,
    Insert: 277,
    Home: 278,
    End: 279,
    PageUp: 280,
    PageDown: 281,
    F1: 282,
    F2: 283,
    F3: 284,
    F4: 285,
    F5: 286,
    F6: 287,
    F7: 288,
    F8: 289,
    F9: 290,
    F10: 291,
    F11: 292,
    F12: 293,
    F13: 294,
    F14: 295,
    F15: 296,
    Alpha0: 48,
    Alpha1: 49,
    Alpha2: 50,
    Alpha3: 51,
    Alpha4: 52,
    Alpha5: 53,
    Alpha6: 54,
    Alpha7: 55,
    Alpha8: 56,
    Alpha9: 57,
    Exclaim: 33,
    DoubleQuote: 34,
    Hash: 35,
    Dollar: 36,
    Percent: 37,
    Ampersand: 38,
    Quote: 39,
    LeftParen: 40,
    RightParen: 41,
    Asterisk: 42,
    Plus: 43,
    Comma: 44,
    Minus: 45,
    Period: 46,
    Slash: 47,
    Colon: 58,
    Semicolon: 59,
    Less: 60,
    Equals: 61,
    Greater: 62,
    Question: 0x3F,
    At: 0x40,
    LeftBracket: 91,
    Backslash: 92,
    RightBracket: 93,
    Caret: 94,
    Underscore: 95,
    BackQuote: 96,
    A: 97,
    B: 98,
    C: 99,
    D: 100,
    E: 101,
    F: 102,
    G: 103,
    H: 104,
    I: 105,
    J: 106,
    K: 107,
    L: 108,
    M: 109,
    N: 110,
    O: 111,
    P: 112,
    Q: 113,
    R: 114,
    S: 115,
    T: 116,
    U: 117,
    V: 118,
    W: 119,
    X: 120,
    Y: 121,
    Z: 122,
    LeftCurlyBracket: 123,
    Pipe: 124,
    RightCurlyBracket: 125,
    Tilde: 126,
    Numlock: 300,
    CapsLock: 301,
    ScrollLock: 302,
    RightShift: 303,
    LeftShift: 304,
    RightControl: 305,
    LeftControl: 306,
    RightAlt: 307,
    LeftAlt: 308,
    LeftMeta: 310,
    LeftCommand: 310,
    LeftApple: 310,
    LeftWindows: 311,
    RightMeta: 309,
    RightCommand: 309,
    RightApple: 309,
    RightWindows: 312,
    AltGr: 313,
    Help: 315,
    Print: 316,
    SysReq: 317,
    Break: 318,
    Menu: 319,
    Mouse0: 323,
    Mouse1: 324,
    Mouse2: 325,
    Mouse3: 326,
    Mouse4: 327,
    Mouse5: 328,
    Mouse6: 329,
    JoystickButton0: 330,
    JoystickButton1: 331,
    JoystickButton2: 332,
    JoystickButton3: 333,
    JoystickButton4: 334,
    JoystickButton5: 335,
    JoystickButton6: 336,
    JoystickButton7: 337,
    JoystickButton8: 338,
    JoystickButton9: 339,
    JoystickButton10: 340,
    JoystickButton11: 341,
    JoystickButton12: 342,
    JoystickButton13: 343,
    JoystickButton14: 344,
    JoystickButton15: 345,
    JoystickButton16: 346,
    JoystickButton17: 347,
    JoystickButton18: 348,
    JoystickButton19: 349,
    Joystick1Button0: 350,
    Joystick1Button1: 351,
    Joystick1Button2: 352,
    Joystick1Button3: 353,
    Joystick1Button4: 354,
    Joystick1Button5: 355,
    Joystick1Button6: 356,
    Joystick1Button7: 357,
    Joystick1Button8: 358,
    Joystick1Button9: 359,
    Joystick1Button10: 360,
    Joystick1Button11: 361,
    Joystick1Button12: 362,
    Joystick1Button13: 363,
    Joystick1Button14: 364,
    Joystick1Button15: 365,
    Joystick1Button16: 366,
    Joystick1Button17: 367,
    Joystick1Button18: 368,
    Joystick1Button19: 369,
    Joystick2Button0: 370,
    Joystick2Button1: 371,
    Joystick2Button2: 372,
    Joystick2Button3: 373,
    Joystick2Button4: 374,
    Joystick2Button5: 375,
    Joystick2Button6: 376,
    Joystick2Button7: 377,
    Joystick2Button8: 378,
    Joystick2Button9: 379,
    Joystick2Button10: 380,
    Joystick2Button11: 381,
    Joystick2Button12: 382,
    Joystick2Button13: 383,
    Joystick2Button14: 384,
    Joystick2Button15: 385,
    Joystick2Button16: 386,
    Joystick2Button17: 387,
    Joystick2Button18: 388,
    Joystick2Button19: 389,
    Joystick3Button0: 390,
    Joystick3Button1: 391,
    Joystick3Button2: 392,
    Joystick3Button3: 393,
    Joystick3Button4: 394,
    Joystick3Button5: 395,
    Joystick3Button6: 396,
    Joystick3Button7: 397,
    Joystick3Button8: 398,
    Joystick3Button9: 399,
    Joystick3Button10: 400,
    Joystick3Button11: 401,
    Joystick3Button12: 402,
    Joystick3Button13: 403,
    Joystick3Button14: 404,
    Joystick3Button15: 405,
    Joystick3Button16: 406,
    Joystick3Button17: 407,
    Joystick3Button18: 408,
    Joystick3Button19: 409,
    Joystick4Button0: 410,
    Joystick4Button1: 411,
    Joystick4Button2: 412,
    Joystick4Button3: 413,
    Joystick4Button4: 414,
    Joystick4Button5: 415,
    Joystick4Button6: 416,
    Joystick4Button7: 417,
    Joystick4Button8: 418,
    Joystick4Button9: 419,
    Joystick4Button10: 420,
    Joystick4Button11: 421,
    Joystick4Button12: 422,
    Joystick4Button13: 423,
    Joystick4Button14: 424,
    Joystick4Button15: 425,
    Joystick4Button16: 426,
    Joystick4Button17: 427,
    Joystick4Button18: 428,
    Joystick4Button19: 429,
    Joystick5Button0: 430,
    Joystick5Button1: 431,
    Joystick5Button2: 432,
    Joystick5Button3: 433,
    Joystick5Button4: 434,
    Joystick5Button5: 435,
    Joystick5Button6: 436,
    Joystick5Button7: 437,
    Joystick5Button8: 438,
    Joystick5Button9: 439,
    Joystick5Button10: 440,
    Joystick5Button11: 441,
    Joystick5Button12: 442,
    Joystick5Button13: 443,
    Joystick5Button14: 444,
    Joystick5Button15: 445,
    Joystick5Button16: 446,
    Joystick5Button17: 447,
    Joystick5Button18: 448,
    Joystick5Button19: 449,
    Joystick6Button0: 450,
    Joystick6Button1: 451,
    Joystick6Button2: 452,
    Joystick6Button3: 453,
    Joystick6Button4: 454,
    Joystick6Button5: 455,
    Joystick6Button6: 456,
    Joystick6Button7: 457,
    Joystick6Button8: 458,
    Joystick6Button9: 459,
    Joystick6Button10: 460,
    Joystick6Button11: 461,
    Joystick6Button12: 462,
    Joystick6Button13: 463,
    Joystick6Button14: 464,
    Joystick6Button15: 465,
    Joystick6Button16: 466,
    Joystick6Button17: 467,
    Joystick6Button18: 468,
    Joystick6Button19: 469,
    Joystick7Button0: 470,
    Joystick7Button1: 471,
    Joystick7Button2: 472,
    Joystick7Button3: 473,
    Joystick7Button4: 474,
    Joystick7Button5: 475,
    Joystick7Button6: 476,
    Joystick7Button7: 477,
    Joystick7Button8: 478,
    Joystick7Button9: 479,
    Joystick7Button10: 480,
    Joystick7Button11: 481,
    Joystick7Button12: 482,
    Joystick7Button13: 483,
    Joystick7Button14: 484,
    Joystick7Button15: 485,
    Joystick7Button16: 486,
    Joystick7Button17: 487,
    Joystick7Button18: 488,
    Joystick7Button19: 489,
    Joystick8Button0: 490,
    Joystick8Button1: 491,
    Joystick8Button2: 492,
    Joystick8Button3: 493,
    Joystick8Button4: 494,
    Joystick8Button5: 495,
    Joystick8Button6: 496,
    Joystick8Button7: 497,
    Joystick8Button8: 498,
    Joystick8Button9: 499,
    Joystick8Button10: 500,
    Joystick8Button11: 501,
    Joystick8Button12: 502,
    Joystick8Button13: 503,
    Joystick8Button14: 504,
    Joystick8Button15: 505,
    Joystick8Button16: 506,
    Joystick8Button17: 507,
    Joystick8Button18: 508,
    Joystick8Button19: 509
}
const PlanetType =
{
    Red: 0,
    Blue: 1,
    Green: 2,
    Yellow: 3,
    Purple: 4,
    Pink: 5,
    Orange: 6,
    Cyan: 7,
    Current: 8,
    Other: 9
};
class Input {
    /**
    * @param {number} key KeyCode
    * @returns {number} Is Key Down
    */
    static getKeyDown(key);
    /**
    * @param {number} key KeyCode
    * @returns {number} Is Key Up
    */
    static getKeyUp(key);
    /**
    * @param {number} key KeyCode
    * @returns {number} Is Key Up Or Down
    */
    static getKey(key);
}
class Overlayer {
    /**
    * @param {Function} obj Anything
    * @returns {number} 0
    */
    static log(obj);
    /**
    * @param {Function} obj Anything
    * @returns {number} 0
    */
    static hit(obj);
    /**
    * @param {Function} obj Anything
    * @returns {number} 0
    */
    static openLevel(obj);
    /**
    * @param {Function} obj Anything
    * @returns {number} 0
    */
    static sceneLoad(obj);
    /**
    * @param {number} planetType PlanetType
    * @returns {Planet} Planet
    */
    static getPlanet(planetType);
    /**
    * @param {string} name Variable Name
    * @returns {any} Variable
    */
    static getGlobalVariable(name);
    /**
    * @param {string} name VariableName
    * @param {any} obj Any Value
    */
    static setGlobalVariable(name, obj);
}
class Vector2 {
    /**
    * @param {number} x x
    * @param {number} y y
    */
    constructor(x, y) {
        this.x = x;
        this.y = y;
    }
    /**
    * Normalize This Vector
    */
    normalize();
}
class Vector3 {
    /**
    * @param {number} x x
    * @param {number} y y
    * @param {number} z z
    */
    constructor(x, y, z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    /**
    * Normalize This Vector
    */
    normalize();
}
class Sprite {
    /**
    * @param {string} path Image Path
    * @returns {Sprite} Image
    */
    static load(path);
}
class GameObject {
    /**
    * @param {Sprite} spr Sprite
    */
    constructor(spr) {
    }
    /**
    * @returns {Vector3} Position
    */
    getPosition();
    /**
    * @param {Vector3} vec3 Position
    */
    setPosition(vec3);
    /**
    * @returns {Color} Color
    */
    getColor();
    /**
    * @param {Color} col Color
    */
    setColor(col);
    /**
    * @returns {Sprite} Sprite
    */
    getSprite();
    /**
    * @param {Sprite} spr Sprite
    */
    setSprite(spr);
}
class Color {
    /**
    * @param {number} r Red
    * @param {number} g Green
    * @param {number} b Blue
    * @param {number} a Alpha
    */
    constructor(r, g, b, a = 1) {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }
}
class Planet {
    /**
    * @param {number} planetType PlanetType
    */
    constructor(planetType) {
    }
    /**
    * @returns {Color} Color
    */
    getColor();
    /**
    * @param {Color} col Color
    */
    setColor(col);
    /**
    * @returns {Sprite} Sprite
    */
    getSprite();
    /**
    * @param {Sprite} spr Sprite
    */
    setSprite(spr);
}
