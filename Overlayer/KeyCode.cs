using Overlayer.Core.JavaScript;
using Overlayer.Core.JavaScript.Library;

namespace Overlayer
{
    public class Kcde : ObjectInstance
    {
        public Kcde(ScriptEngine engine) : base(engine) => PopulateFields();
        [JSField]
        public static int None = 0;
        [JSField]
        public static int Backspace = 8;
        [JSField]
        public static int Tab = 9;
        [JSField]
        public static int Clear = 12;
        [JSField]
        public static int Return = 13;
        [JSField]
        public static int Pause = 19;
        [JSField]
        public static int Escape = 27;
        [JSField]
        public static int Space = 32;
        [JSField]
        public static int Exclaim = 33;
        [JSField]
        public static int DoubleQuote = 34;
        [JSField]
        public static int Hash = 35;
        [JSField]
        public static int Dollar = 36;
        [JSField]
        public static int Percent = 37;
        [JSField]
        public static int Ampersand = 38;
        [JSField]
        public static int Quote = 39;
        [JSField]
        public static int LeftParen = 40;
        [JSField]
        public static int RightParen = 41;
        [JSField]
        public static int Asterisk = 42;
        [JSField]
        public static int Plus = 43;
        [JSField]
        public static int Comma = 44;
        [JSField]
        public static int Minus = 45;
        [JSField]
        public static int Period = 46;
        [JSField]
        public static int Slash = 47;
        [JSField]
        public static int Alpha0 = 48;
        [JSField]
        public static int Alpha1 = 49;
        [JSField]
        public static int Alpha2 = 50;
        [JSField]
        public static int Alpha3 = 51;
        [JSField]
        public static int Alpha4 = 52;
        [JSField]
        public static int Alpha5 = 53;
        [JSField]
        public static int Alpha6 = 54;
        [JSField]
        public static int Alpha7 = 55;
        [JSField]
        public static int Alpha8 = 56;
        [JSField]
        public static int Alpha9 = 57;
        [JSField]
        public static int Colon = 58;
        [JSField]
        public static int Semicolon = 59;
        [JSField]
        public static int Less = 60;
        [JSField]
        public static new int Equals = 61;
        [JSField]
        public static int Greater = 62;
        [JSField]
        public static int Question = 63;
        [JSField]
        public static int At = 64;
        [JSField]
        public static int LeftBracket = 91;
        [JSField]
        public static int Backslash = 92;
        [JSField]
        public static int RightBracket = 93;
        [JSField]
        public static int Caret = 94;
        [JSField]
        public static int Underscore = 95;
        [JSField]
        public static int BackQuote = 96;
        [JSField]
        public static int A = 97;
        [JSField]
        public static int B = 98;
        [JSField]
        public static int C = 99;
        [JSField]
        public static int D = 100;
        [JSField]
        public static int E = 101;
        [JSField]
        public static int F = 102;
        [JSField]
        public static int G = 103;
        [JSField]
        public static int H = 104;
        [JSField]
        public static int I = 105;
        [JSField]
        public static int J = 106;
        [JSField]
        public static int K = 107;
        [JSField]
        public static int L = 108;
        [JSField]
        public static int M = 109;
        [JSField]
        public static int N = 110;
        [JSField]
        public static int O = 111;
        [JSField]
        public static int P = 112;
        [JSField]
        public static int Q = 113;
        [JSField]
        public static int R = 114;
        [JSField]
        public static int S = 115;
        [JSField]
        public static int T = 116;
        [JSField]
        public static int U = 117;
        [JSField]
        public static int V = 118;
        [JSField]
        public static int W = 119;
        [JSField]
        public static int X = 120;
        [JSField]
        public static int Y = 121;
        [JSField]
        public static int Z = 122;
        [JSField]
        public static int LeftCurlyBracket = 123;
        [JSField]
        public static int Pipe = 124;
        [JSField]
        public static int RightCurlyBracket = 125;
        [JSField]
        public static int Tilde = 126;
        [JSField]
        public static new int Delete = 127;
        [JSField]
        public static int Keypad0 = 256;
        [JSField]
        public static int Keypad1 = 257;
        [JSField]
        public static int Keypad2 = 258;
        [JSField]
        public static int Keypad3 = 259;
        [JSField]
        public static int Keypad4 = 260;
        [JSField]
        public static int Keypad5 = 261;
        [JSField]
        public static int Keypad6 = 262;
        [JSField]
        public static int Keypad7 = 263;
        [JSField]
        public static int Keypad8 = 264;
        [JSField]
        public static int Keypad9 = 265;
        [JSField]
        public static int KeypadPeriod = 266;
        [JSField]
        public static int KeypadDivide = 267;
        [JSField]
        public static int KeypadMultiply = 268;
        [JSField]
        public static int KeypadMinus = 269;
        [JSField]
        public static int KeypadPlus = 270;
        [JSField]
        public static int KeypadEnter = 271;
        [JSField]
        public static int KeypadEquals = 272;
        [JSField]
        public static int UpArrow = 273;
        [JSField]
        public static int DownArrow = 274;
        [JSField]
        public static int RightArrow = 275;
        [JSField]
        public static int LeftArrow = 276;
        [JSField]
        public static int Insert = 277;
        [JSField]
        public static int Home = 278;
        [JSField]
        public static int End = 279;
        [JSField]
        public static int PageUp = 280;
        [JSField]
        public static int PageDown = 281;
        [JSField]
        public static int F1 = 282;
        [JSField]
        public static int F2 = 283;
        [JSField]
        public static int F3 = 284;
        [JSField]
        public static int F4 = 285;
        [JSField]
        public static int F5 = 286;
        [JSField]
        public static int F6 = 287;
        [JSField]
        public static int F7 = 288;
        [JSField]
        public static int F8 = 289;
        [JSField]
        public static int F9 = 290;
        [JSField]
        public static int F10 = 291;
        [JSField]
        public static int F11 = 292;
        [JSField]
        public static int F12 = 293;
        [JSField]
        public static int F13 = 294;
        [JSField]
        public static int F14 = 295;
        [JSField]
        public static int F15 = 296;
        [JSField]
        public static int Numlock = 300;
        [JSField]
        public static int CapsLock = 301;
        [JSField]
        public static int ScrollLock = 302;
        [JSField]
        public static int RightShift = 303;
        [JSField]
        public static int LeftShift = 304;
        [JSField]
        public static int RightControl = 305;
        [JSField]
        public static int LeftControl = 306;
        [JSField]
        public static int RightAlt = 307;
        [JSField]
        public static int LeftAlt = 308;
        [JSField]
        public static int RightMeta = 309;
        [JSField]
        public static int LeftApple = 310;
        [JSField]
        public static int LeftWindows = 311;
        [JSField]
        public static int RightWindows = 312;
        [JSField]
        public static int AltGr = 313;
        [JSField]
        public static int Help = 315;
        [JSField]
        public static int Print = 316;
        [JSField]
        public static int SysReq = 317;
        [JSField]
        public static int Break = 318;
        [JSField]
        public static int Menu = 319;
        [JSField]
        public static int Mouse0 = 323;
        [JSField]
        public static int Mouse1 = 324;
        [JSField]
        public static int Mouse2 = 325;
        [JSField]
        public static int Mouse3 = 326;
        [JSField]
        public static int Mouse4 = 327;
        [JSField]
        public static int Mouse5 = 328;
        [JSField]
        public static int Mouse6 = 329;
        [JSField]
        public static int JoystickButton0 = 330;
        [JSField]
        public static int JoystickButton1 = 331;
        [JSField]
        public static int JoystickButton2 = 332;
        [JSField]
        public static int JoystickButton3 = 333;
        [JSField]
        public static int JoystickButton4 = 334;
        [JSField]
        public static int JoystickButton5 = 335;
        [JSField]
        public static int JoystickButton6 = 336;
        [JSField]
        public static int JoystickButton7 = 337;
        [JSField]
        public static int JoystickButton8 = 338;
        [JSField]
        public static int JoystickButton9 = 339;
        [JSField]
        public static int JoystickButton10 = 340;
        [JSField]
        public static int JoystickButton11 = 341;
        [JSField]
        public static int JoystickButton12 = 342;
        [JSField]
        public static int JoystickButton13 = 343;
        [JSField]
        public static int JoystickButton14 = 344;
        [JSField]
        public static int JoystickButton15 = 345;
        [JSField]
        public static int JoystickButton16 = 346;
        [JSField]
        public static int JoystickButton17 = 347;
        [JSField]
        public static int JoystickButton18 = 348;
        [JSField]
        public static int JoystickButton19 = 349;
        [JSField]
        public static int Joystick1Button0 = 350;
        [JSField]
        public static int Joystick1Button1 = 351;
        [JSField]
        public static int Joystick1Button2 = 352;
        [JSField]
        public static int Joystick1Button3 = 353;
        [JSField]
        public static int Joystick1Button4 = 354;
        [JSField]
        public static int Joystick1Button5 = 355;
        [JSField]
        public static int Joystick1Button6 = 356;
        [JSField]
        public static int Joystick1Button7 = 357;
        [JSField]
        public static int Joystick1Button8 = 358;
        [JSField]
        public static int Joystick1Button9 = 359;
        [JSField]
        public static int Joystick1Button10 = 360;
        [JSField]
        public static int Joystick1Button11 = 361;
        [JSField]
        public static int Joystick1Button12 = 362;
        [JSField]
        public static int Joystick1Button13 = 363;
        [JSField]
        public static int Joystick1Button14 = 364;
        [JSField]
        public static int Joystick1Button15 = 365;
        [JSField]
        public static int Joystick1Button16 = 366;
        [JSField]
        public static int Joystick1Button17 = 367;
        [JSField]
        public static int Joystick1Button18 = 368;
        [JSField]
        public static int Joystick1Button19 = 369;
        [JSField]
        public static int Joystick2Button0 = 370;
        [JSField]
        public static int Joystick2Button1 = 371;
        [JSField]
        public static int Joystick2Button2 = 372;
        [JSField]
        public static int Joystick2Button3 = 373;
        [JSField]
        public static int Joystick2Button4 = 374;
        [JSField]
        public static int Joystick2Button5 = 375;
        [JSField]
        public static int Joystick2Button6 = 376;
        [JSField]
        public static int Joystick2Button7 = 377;
        [JSField]
        public static int Joystick2Button8 = 378;
        [JSField]
        public static int Joystick2Button9 = 379;
        [JSField]
        public static int Joystick2Button10 = 380;
        [JSField]
        public static int Joystick2Button11 = 381;
        [JSField]
        public static int Joystick2Button12 = 382;
        [JSField]
        public static int Joystick2Button13 = 383;
        [JSField]
        public static int Joystick2Button14 = 384;
        [JSField]
        public static int Joystick2Button15 = 385;
        [JSField]
        public static int Joystick2Button16 = 386;
        [JSField]
        public static int Joystick2Button17 = 387;
        [JSField]
        public static int Joystick2Button18 = 388;
        [JSField]
        public static int Joystick2Button19 = 389;
        [JSField]
        public static int Joystick3Button0 = 390;
        [JSField]
        public static int Joystick3Button1 = 391;
        [JSField]
        public static int Joystick3Button2 = 392;
        [JSField]
        public static int Joystick3Button3 = 393;
        [JSField]
        public static int Joystick3Button4 = 394;
        [JSField]
        public static int Joystick3Button5 = 395;
        [JSField]
        public static int Joystick3Button6 = 396;
        [JSField]
        public static int Joystick3Button7 = 397;
        [JSField]
        public static int Joystick3Button8 = 398;
        [JSField]
        public static int Joystick3Button9 = 399;
        [JSField]
        public static int Joystick3Button10 = 400;
        [JSField]
        public static int Joystick3Button11 = 401;
        [JSField]
        public static int Joystick3Button12 = 402;
        [JSField]
        public static int Joystick3Button13 = 403;
        [JSField]
        public static int Joystick3Button14 = 404;
        [JSField]
        public static int Joystick3Button15 = 405;
        [JSField]
        public static int Joystick3Button16 = 406;
        [JSField]
        public static int Joystick3Button17 = 407;
        [JSField]
        public static int Joystick3Button18 = 408;
        [JSField]
        public static int Joystick3Button19 = 409;
        [JSField]
        public static int Joystick4Button0 = 410;
        [JSField]
        public static int Joystick4Button1 = 411;
        [JSField]
        public static int Joystick4Button2 = 412;
        [JSField]
        public static int Joystick4Button3 = 413;
        [JSField]
        public static int Joystick4Button4 = 414;
        [JSField]
        public static int Joystick4Button5 = 415;
        [JSField]
        public static int Joystick4Button6 = 416;
        [JSField]
        public static int Joystick4Button7 = 417;
        [JSField]
        public static int Joystick4Button8 = 418;
        [JSField]
        public static int Joystick4Button9 = 419;
        [JSField]
        public static int Joystick4Button10 = 420;
        [JSField]
        public static int Joystick4Button11 = 421;
        [JSField]
        public static int Joystick4Button12 = 422;
        [JSField]
        public static int Joystick4Button13 = 423;
        [JSField]
        public static int Joystick4Button14 = 424;
        [JSField]
        public static int Joystick4Button15 = 425;
        [JSField]
        public static int Joystick4Button16 = 426;
        [JSField]
        public static int Joystick4Button17 = 427;
        [JSField]
        public static int Joystick4Button18 = 428;
        [JSField]
        public static int Joystick4Button19 = 429;
        [JSField]
        public static int Joystick5Button0 = 430;
        [JSField]
        public static int Joystick5Button1 = 431;
        [JSField]
        public static int Joystick5Button2 = 432;
        [JSField]
        public static int Joystick5Button3 = 433;
        [JSField]
        public static int Joystick5Button4 = 434;
        [JSField]
        public static int Joystick5Button5 = 435;
        [JSField]
        public static int Joystick5Button6 = 436;
        [JSField]
        public static int Joystick5Button7 = 437;
        [JSField]
        public static int Joystick5Button8 = 438;
        [JSField]
        public static int Joystick5Button9 = 439;
        [JSField]
        public static int Joystick5Button10 = 440;
        [JSField]
        public static int Joystick5Button11 = 441;
        [JSField]
        public static int Joystick5Button12 = 442;
        [JSField]
        public static int Joystick5Button13 = 443;
        [JSField]
        public static int Joystick5Button14 = 444;
        [JSField]
        public static int Joystick5Button15 = 445;
        [JSField]
        public static int Joystick5Button16 = 446;
        [JSField]
        public static int Joystick5Button17 = 447;
        [JSField]
        public static int Joystick5Button18 = 448;
        [JSField]
        public static int Joystick5Button19 = 449;
        [JSField]
        public static int Joystick6Button0 = 450;
        [JSField]
        public static int Joystick6Button1 = 451;
        [JSField]
        public static int Joystick6Button2 = 452;
        [JSField]
        public static int Joystick6Button3 = 453;
        [JSField]
        public static int Joystick6Button4 = 454;
        [JSField]
        public static int Joystick6Button5 = 455;
        [JSField]
        public static int Joystick6Button6 = 456;
        [JSField]
        public static int Joystick6Button7 = 457;
        [JSField]
        public static int Joystick6Button8 = 458;
        [JSField]
        public static int Joystick6Button9 = 459;
        [JSField]
        public static int Joystick6Button10 = 460;
        [JSField]
        public static int Joystick6Button11 = 461;
        [JSField]
        public static int Joystick6Button12 = 462;
        [JSField]
        public static int Joystick6Button13 = 463;
        [JSField]
        public static int Joystick6Button14 = 464;
        [JSField]
        public static int Joystick6Button15 = 465;
        [JSField]
        public static int Joystick6Button16 = 466;
        [JSField]
        public static int Joystick6Button17 = 467;
        [JSField]
        public static int Joystick6Button18 = 468;
        [JSField]
        public static int Joystick6Button19 = 469;
        [JSField]
        public static int Joystick7Button0 = 470;
        [JSField]
        public static int Joystick7Button1 = 471;
        [JSField]
        public static int Joystick7Button2 = 472;
        [JSField]
        public static int Joystick7Button3 = 473;
        [JSField]
        public static int Joystick7Button4 = 474;
        [JSField]
        public static int Joystick7Button5 = 475;
        [JSField]
        public static int Joystick7Button6 = 476;
        [JSField]
        public static int Joystick7Button7 = 477;
        [JSField]
        public static int Joystick7Button8 = 478;
        [JSField]
        public static int Joystick7Button9 = 479;
        [JSField]
        public static int Joystick7Button10 = 480;
        [JSField]
        public static int Joystick7Button11 = 481;
        [JSField]
        public static int Joystick7Button12 = 482;
        [JSField]
        public static int Joystick7Button13 = 483;
        [JSField]
        public static int Joystick7Button14 = 484;
        [JSField]
        public static int Joystick7Button15 = 485;
        [JSField]
        public static int Joystick7Button16 = 486;
        [JSField]
        public static int Joystick7Button17 = 487;
        [JSField]
        public static int Joystick7Button18 = 488;
        [JSField]
        public static int Joystick7Button19 = 489;
        [JSField]
        public static int Joystick8Button0 = 490;
        [JSField]
        public static int Joystick8Button1 = 491;
        [JSField]
        public static int Joystick8Button2 = 492;
        [JSField]
        public static int Joystick8Button3 = 493;
        [JSField]
        public static int Joystick8Button4 = 494;
        [JSField]
        public static int Joystick8Button5 = 495;
        [JSField]
        public static int Joystick8Button6 = 496;
        [JSField]
        public static int Joystick8Button7 = 497;
        [JSField]
        public static int Joystick8Button8 = 498;
        [JSField]
        public static int Joystick8Button9 = 499;
        [JSField]
        public static int Joystick8Button10 = 500;
        [JSField]
        public static int Joystick8Button11 = 501;
        [JSField]
        public static int Joystick8Button12 = 502;
        [JSField]
        public static int Joystick8Button13 = 503;
        [JSField]
        public static int Joystick8Button14 = 504;
        [JSField]
        public static int Joystick8Button15 = 505;
        [JSField]
        public static int Joystick8Button16 = 506;
        [JSField]
        public static int Joystick8Button17 = 507;
        [JSField]
        public static int Joystick8Button18 = 508;
        [JSField]
        public static int Joystick8Button19 = 509;
    }
}
