using Overlayer.MapParser.Types;
using Overlayer.MapParser.Decorations;
using Overlayer.MapParser.Helpers;
using JSON;
using System.Collections.Generic;

namespace Overlayer.MapParser.Actions
{
    public static class ActionUtils
    {
        public static JsonNode InitNode(LevelEventType evtType, bool active)
        {
            JsonNode node = JsonNode.Empty;
            node.Inline = true;
            node["eventType"] = evtType.ToString();
            if (!active)
                node["active"] = false;
            return node;
        }
        static bool CheckIsNull(JsonNode node)
            => node is JsonLazyCreator;
        public static Action ParseAction(JsonNode node)
        {
            switch (node["eventType"].ToString().TrimLR().Parse<LevelEventType>())
            {
                case LevelEventType.AnimateTrack:
                    AnimateTrack animatetrack = new AnimateTrack();
                    if (!CheckIsNull(node["trackAnimation"]))
                        animatetrack.trackAnimation = node["trackAnimation"].ToString().TrimLR().Parse<TrackAnimationType>();
                    if (!CheckIsNull(node["beatsAhead"]))
                        animatetrack.beatsAhead = node["beatsAhead"].AsInt;
                    if (!CheckIsNull(node["trackDisappearAnimation"]))
                        animatetrack.trackDisappearAnimation = node["trackDisappearAnimation"].ToString().TrimLR().Parse<TrackAnimationType2>();
                    if (!CheckIsNull(node["beatsBehind"]))
                        animatetrack.beatsBehind = node["beatsBehind"].AsInt;
                    if (!CheckIsNull(node["active"]))
                        animatetrack.active = node["active"].AsBool;
                    return animatetrack;
                case LevelEventType.AutoPlayTiles:
                    AutoPlayTiles autoplaytiles = new AutoPlayTiles();
                    if (!CheckIsNull(node["enabled"]))
                        autoplaytiles.enabled = node["enabled"].ToString().TrimLR().Parse<Toggle>();
                    if (!CheckIsNull(node["safetyTiles"]))
                        autoplaytiles.safetyTiles = node["safetyTiles"].ToString().TrimLR().Parse<Toggle>();
                    if (!CheckIsNull(node["active"]))
                        autoplaytiles.active = node["active"].AsBool;
                    return autoplaytiles;
                case LevelEventType.Bloom:
                    Bloom bloom = new Bloom();
                    if (!CheckIsNull(node["enabled"]))
                        bloom.enabled = node["enabled"].ToString().TrimLR().Parse<Toggle>();
                    if (!CheckIsNull(node["threshold"]))
                        bloom.threshold = node["threshold"].AsDouble;
                    if (!CheckIsNull(node["intensity"]))
                        bloom.intensity = node["intensity"].AsDouble;
                    if (!CheckIsNull(node["color"]))
                        bloom.color = node["color"].ToString().TrimLR();
                    if (!CheckIsNull(node["duration"]))
                        bloom.duration = node["duration"].AsDouble;
                    if (!CheckIsNull(node["ease"]))
                        bloom.ease = node["ease"].ToString().TrimLR().Parse<Ease>();
                    if (!CheckIsNull(node["angleOffset"]))
                        bloom.angleOffset = node["angleOffset"].AsDouble;
                    if (!CheckIsNull(node["eventTag"]))
                        bloom.eventTag = node["eventTag"].ToString().TrimLR();
                    if (!CheckIsNull(node["active"]))
                        bloom.active = node["active"].AsBool;
                    return bloom;
                case LevelEventType.Bookmark:
                    Bookmark bookmark = new Bookmark();
                    if (!CheckIsNull(node["active"]))
                        bookmark.active = node["active"].AsBool;
                    return bookmark;
                case LevelEventType.ChangeTrack:
                    ChangeTrack changetrack = new ChangeTrack();
                    if (!CheckIsNull(node["trackColorType"]))
                        changetrack.trackColorType = node["trackColorType"].ToString().TrimLR().Parse<TrackColorType>();
                    if (!CheckIsNull(node["trackColor"]))
                        changetrack.trackColor = node["trackColor"].ToString().TrimLR();
                    if (!CheckIsNull(node["secondaryTrackColor"]))
                        changetrack.secondaryTrackColor = node["secondaryTrackColor"].ToString().TrimLR();
                    if (!CheckIsNull(node["trackColorAnimDuration"]))
                        changetrack.trackColorAnimDuration = node["trackColorAnimDuration"].AsDouble;
                    if (!CheckIsNull(node["trackColorPulse"]))
                        changetrack.trackColorPulse = node["trackColorPulse"].ToString().TrimLR().Parse<TrackColorPulse>();
                    if (!CheckIsNull(node["trackPulseLength"]))
                        changetrack.trackPulseLength = node["trackPulseLength"].AsDouble;
                    if (!CheckIsNull(node["trackStyle"]))
                        changetrack.trackStyle = node["trackStyle"].ToString().TrimLR().Parse<TrackStyle>();
                    if (!CheckIsNull(node["trackAnimation"]))
                        changetrack.trackAnimation = node["trackAnimation"].ToString().TrimLR().Parse<TrackAnimationType>();
                    if (!CheckIsNull(node["beatsAhead"]))
                        changetrack.beatsAhead = node["beatsAhead"].AsInt;
                    if (!CheckIsNull(node["trackDisappearAnimation"]))
                        changetrack.trackDisappearAnimation = node["trackDisappearAnimation"].ToString().TrimLR().Parse<TrackAnimationType2>();
                    if (!CheckIsNull(node["beatsBehind"]))
                        changetrack.beatsBehind = node["beatsBehind"].AsInt;
                    if (!CheckIsNull(node["active"]))
                        changetrack.active = node["active"].AsBool;
                    return changetrack;
                case LevelEventType.Checkpoint:
                    Checkpoint checkpoint = new Checkpoint();
                    if (!CheckIsNull(node["tileOffset"]))
                        checkpoint.tileOffset = node["tileOffset"].AsInt;
                    if (!CheckIsNull(node["active"]))
                        checkpoint.active = node["active"].AsBool;
                    return checkpoint;
                case LevelEventType.ColorTrack:
                    ColorTrack colortrack = new ColorTrack();
                    if (!CheckIsNull(node["trackColorType"]))
                        colortrack.trackColorType = node["trackColorType"].ToString().TrimLR().Parse<TrackColorType>();
                    if (!CheckIsNull(node["trackColor"]))
                        colortrack.trackColor = node["trackColor"].ToString().TrimLR();
                    if (!CheckIsNull(node["secondaryTrackColor"]))
                        colortrack.secondaryTrackColor = node["secondaryTrackColor"].ToString().TrimLR();
                    if (!CheckIsNull(node["trackColorAnimDuration"]))
                        colortrack.trackColorAnimDuration = node["trackColorAnimDuration"].AsDouble;
                    if (!CheckIsNull(node["trackColorPulse"]))
                        colortrack.trackColorPulse = node["trackColorPulse"].ToString().TrimLR().Parse<TrackColorPulse>();
                    if (!CheckIsNull(node["trackPulseLength"]))
                        colortrack.trackPulseLength = node["trackPulseLength"].AsDouble;
                    if (!CheckIsNull(node["trackStyle"]))
                        colortrack.trackStyle = node["trackStyle"].ToString().TrimLR().Parse<TrackStyle>();
                    if (!CheckIsNull(node["trackTexture"]))
                        colortrack.trackTexture = node["trackTexture"].ToString().TrimLR();
                    if (!CheckIsNull(node["trackTextureScale"]))
                        colortrack.trackTextureScale = node["trackTextureScale"].AsDouble;
                    if (!CheckIsNull(node["active"]))
                        colortrack.active = node["active"].AsBool;
                    return colortrack;
                case LevelEventType.CustomBackground:
                    CustomBackground custombackground = new CustomBackground();
                    if (!CheckIsNull(node["color"]))
                        custombackground.color = node["color"].ToString().TrimLR();
                    if (!CheckIsNull(node["bgImage"]))
                        custombackground.bgImage = node["bgImage"].ToString().TrimLR();
                    if (!CheckIsNull(node["imageColor"]))
                        custombackground.imageColor = node["imageColor"].ToString().TrimLR();
                    if (!CheckIsNull(node["parallax"]))
                        custombackground.parallax = Vector2.FromNode(node["parallax"]);
                    if (!CheckIsNull(node["bgDisplayMode"]))
                        custombackground.bgDisplayMode = node["bgDisplayMode"].ToString().TrimLR().Parse<BgDisplayMode>();
                    if (!CheckIsNull(node["lockRot"]))
                        custombackground.lockRot = node["lockRot"].ToString().TrimLR().Parse<Toggle>();
                    if (!CheckIsNull(node["loopBG"]))
                        custombackground.loopBG = node["loopBG"].ToString().TrimLR().Parse<Toggle>();
                    if (!CheckIsNull(node["unscaledSize"]))
                        custombackground.unscaledSize = node["unscaledSize"].AsDouble;
                    if (!CheckIsNull(node["angleOffset"]))
                        custombackground.angleOffset = node["angleOffset"].AsDouble;
                    if (!CheckIsNull(node["eventTag"]))
                        custombackground.eventTag = node["eventTag"].ToString().TrimLR();
                    if (!CheckIsNull(node["active"]))
                        custombackground.active = node["active"].AsBool;
                    return custombackground;
                case LevelEventType.EditorComment:
                    EditorComment editorcomment = new EditorComment();
                    if (!CheckIsNull(node["comment"]))
                        editorcomment.comment = node["comment"].ToString().TrimLR();
                    if (!CheckIsNull(node["active"]))
                        editorcomment.active = node["active"].AsBool;
                    return editorcomment;
                case LevelEventType.Flash:
                    Flash flash = new Flash();
                    if (!CheckIsNull(node["duration"]))
                        flash.duration = node["duration"].AsDouble;
                    if (!CheckIsNull(node["plane"]))
                        flash.plane = node["plane"].ToString().TrimLR().Parse<FlashPlane>();
                    if (!CheckIsNull(node["startColor"]))
                        flash.startColor = node["startColor"].ToString().TrimLR();
                    if (!CheckIsNull(node["startOpacity"]))
                        flash.startOpacity = node["startOpacity"].AsInt;
                    if (!CheckIsNull(node["endColor"]))
                        flash.endColor = node["endColor"].ToString().TrimLR();
                    if (!CheckIsNull(node["endOpacity"]))
                        flash.endOpacity = node["endOpacity"].AsInt;
                    if (!CheckIsNull(node["angleOffset"]))
                        flash.angleOffset = node["angleOffset"].AsDouble;
                    if (!CheckIsNull(node["ease"]))
                        flash.ease = node["ease"].ToString().TrimLR().Parse<Ease>();
                    if (!CheckIsNull(node["eventTag"]))
                        flash.eventTag = node["eventTag"].ToString().TrimLR();
                    if (!CheckIsNull(node["active"]))
                        flash.active = node["active"].AsBool;
                    return flash;
                case LevelEventType.FreeRoam:
                    FreeRoam freeroam = new FreeRoam();
                    if (!CheckIsNull(node["duration"]))
                        freeroam.duration = node["duration"].AsDouble;
                    if (!CheckIsNull(node["size"]))
                        freeroam.size = Vector2.FromNode(node["size"]);
                    if (!CheckIsNull(node["positionOffset"]))
                        freeroam.positionOffset = Vector2.FromNode(node["positionOffset"]);
                    if (!CheckIsNull(node["outTime"]))
                        freeroam.outTime = node["outTime"].AsDouble;
                    if (!CheckIsNull(node["outEase"]))
                        freeroam.outEase = node["outEase"].ToString().TrimLR().Parse<Ease>();
                    if (!CheckIsNull(node["hitsoundOnBeats"]))
                        freeroam.hitsoundOnBeats = node["hitsoundOnBeats"].ToString().TrimLR().Parse<HitSound>();
                    if (!CheckIsNull(node["hitsoundOffBeats"]))
                        freeroam.hitsoundOffBeats = node["hitsoundOffBeats"].ToString().TrimLR().Parse<HitSound>();
                    if (!CheckIsNull(node["countdownTicks"]))
                        freeroam.countdownTicks = node["countdownTicks"].AsInt;
                    if (!CheckIsNull(node["angleCorrectionDir"]))
                        freeroam.angleCorrectionDir = node["angleCorrectionDir"].AsDouble;
                    if (!CheckIsNull(node["active"]))
                        freeroam.active = node["active"].AsBool;
                    return freeroam;
                case LevelEventType.FreeRoamRemove:
                    FreeRoamRemove freeroamremove = new FreeRoamRemove();
                    if (!CheckIsNull(node["position"]))
                        freeroamremove.position = Vector2.FromNode(node["position"]);
                    if (!CheckIsNull(node["size"]))
                        freeroamremove.size = Vector2.FromNode(node["size"]);
                    if (!CheckIsNull(node["active"]))
                        freeroamremove.active = node["active"].AsBool;
                    return freeroamremove;
                case LevelEventType.FreeRoamTwirl:
                    FreeRoamTwirl freeroamtwirl = new FreeRoamTwirl();
                    if (!CheckIsNull(node["position"]))
                        freeroamtwirl.position = Vector2.FromNode(node["position"]);
                    if (!CheckIsNull(node["active"]))
                        freeroamtwirl.active = node["active"].AsBool;
                    return freeroamtwirl;
                case LevelEventType.HallOfMirrors:
                    HallOfMirrors hallofmirrors = new HallOfMirrors();
                    if (!CheckIsNull(node["enabled"]))
                        hallofmirrors.enabled = node["enabled"].ToString().TrimLR().Parse<Toggle>();
                    if (!CheckIsNull(node["angleOffset"]))
                        hallofmirrors.angleOffset = node["angleOffset"].AsDouble;
                    if (!CheckIsNull(node["eventTag"]))
                        hallofmirrors.eventTag = node["eventTag"].ToString().TrimLR();
                    if (!CheckIsNull(node["active"]))
                        hallofmirrors.active = node["active"].AsBool;
                    return hallofmirrors;
                case LevelEventType.Hide:
                    Hide hide = new Hide();
                    if (!CheckIsNull(node["hideJudgment"]))
                        hide.hideJudgment = node["hideJudgment"].ToString().TrimLR().Parse<Toggle>();
                    if (!CheckIsNull(node["hideTileIcon"]))
                        hide.hideTileIcon = node["hideTileIcon"].ToString().TrimLR().Parse<Toggle>();
                    if (!CheckIsNull(node["active"]))
                        hide.active = node["active"].AsBool;
                    return hide;
                case LevelEventType.Hold:
                    Hold hold = new Hold();
                    if (!CheckIsNull(node["duration"]))
                        hold.duration = node["duration"].AsInt;
                    if (!CheckIsNull(node["distanceMultiplier"]))
                        hold.distanceMultiplier = node["distanceMultiplier"].AsInt;
                    if (!CheckIsNull(node["landingAnimation"]))
                        hold.landingAnimation = node["landingAnimation"].ToString().TrimLR().Parse<Toggle>();
                    if (!CheckIsNull(node["active"]))
                        hold.active = node["active"].AsBool;
                    return hold;
                case LevelEventType.MoveCamera:
                    MoveCamera movecamera = new MoveCamera();
                    if (!CheckIsNull(node["duration"]))
                        movecamera.duration = node["duration"].AsDouble;
                    if (!CheckIsNull(node["relativeTo"]))
                        movecamera.relativeTo = node["relativeTo"].ToString().TrimLR().Parse<CamMovementType>();
                    if (!CheckIsNull(node["position"]))
                        movecamera.position = Vector2.FromNode(node["position"]);
                    if (!CheckIsNull(node["rotation"]))
                        movecamera.rotation = node["rotation"].AsDouble;
                    if (!CheckIsNull(node["zoom"]))
                        movecamera.zoom = node["zoom"].AsInt;
                    if (!CheckIsNull(node["angleOffset"]))
                        movecamera.angleOffset = node["angleOffset"].AsDouble;
                    if (!CheckIsNull(node["ease"]))
                        movecamera.ease = node["ease"].ToString().TrimLR().Parse<Ease>();
                    if (!CheckIsNull(node["dontDisable"]))
                        movecamera.dontDisable = node["dontDisable"].ToString().TrimLR().Parse<Toggle>();
                    if (!CheckIsNull(node["minVfxOnly"]))
                        movecamera.minVfxOnly = node["minVfxOnly"].ToString().TrimLR().Parse<Toggle>();
                    if (!CheckIsNull(node["eventTag"]))
                        movecamera.eventTag = node["eventTag"].ToString().TrimLR();
                    if (!CheckIsNull(node["active"]))
                        movecamera.active = node["active"].AsBool;
                    return movecamera;
                case LevelEventType.MoveDecorations:
                    MoveDecorations movedecorations = new MoveDecorations();
                    if (!CheckIsNull(node["duration"]))
                        movedecorations.duration = node["duration"].AsDouble;
                    if (!CheckIsNull(node["tag"]))
                        movedecorations.tag = node["tag"].ToString().TrimLR();
                    if (!CheckIsNull(node["decorationImage"]))
                        movedecorations.decorationImage = node["decorationImage"].ToString().TrimLR();
                    if (!CheckIsNull(node["positionOffset"]))
                        movedecorations.positionOffset = Vector2.FromNode(node["positionOffset"]);
                    if (!CheckIsNull(node["rotationOffset"]))
                        movedecorations.rotationOffset = node["rotationOffset"].AsDouble;
                    if (!CheckIsNull(node["scale"]))
                        movedecorations.scale = node["scale"].AsDouble;
                    if (!CheckIsNull(node["color"]))
                        movedecorations.color = node["color"].ToString().TrimLR();
                    if (!CheckIsNull(node["depth"]))
                        movedecorations.depth = node["depth"].AsInt;
                    if (!CheckIsNull(node["parallax"]))
                        movedecorations.parallax = Vector2.FromNode(node["parallax"]);
                    if (!CheckIsNull(node["angleOffset"]))
                        movedecorations.angleOffset = node["angleOffset"].AsDouble;
                    if (!CheckIsNull(node["ease"]))
                        movedecorations.ease = node["ease"].ToString().TrimLR().Parse<Ease>();
                    if (!CheckIsNull(node["eventTag"]))
                        movedecorations.eventTag = node["eventTag"].ToString().TrimLR();
                    if (!CheckIsNull(node["active"]))
                        movedecorations.active = node["active"].AsBool;
                    return movedecorations;
                case LevelEventType.MoveTrack:
                    MoveTrack movetrack = new MoveTrack();
                    if (!CheckIsNull(node["startTile"]))
                    {
                        JsonArray starttileArr = node["startTile"].AsArray;
                        (int, TileRelativeTo) starttile = (starttileArr[0].AsInt, starttileArr[1].ToString().TrimLR().Parse<TileRelativeTo>());
                        movetrack.startTile = starttile;
                    }
                    if (!CheckIsNull(node["endTile"]))
                    {
                        JsonArray endtile2Arr = node["endTile"].AsArray;
                        (int, TileRelativeTo) endtile2 = (endtile2Arr[0].AsInt, endtile2Arr[1].ToString().TrimLR().Parse<TileRelativeTo>());
                        movetrack.endTile = endtile2;
                    }
                    if (!CheckIsNull(node["gapLength"]))
                        movetrack.gapLength = node["gapLength"].AsDouble;
                    if (!CheckIsNull(node["duration"]))
                        movetrack.duration = node["duration"].AsDouble;
                    if (!CheckIsNull(node["positionOffset"]))
                        movetrack.positionOffset = Vector2.FromNode(node["positionOffset"]);
                    if (!CheckIsNull(node["rotationOffset"]))
                        movetrack.rotationOffset = node["rotationOffset"].AsDouble;
                    if (!CheckIsNull(node["scale"]))
                        movetrack.scale = node["scale"].AsDouble;
                    if (!CheckIsNull(node["opacity"]))
                        movetrack.opacity = node["opacity"].AsInt;
                    if (!CheckIsNull(node["angleOffset"]))
                        movetrack.angleOffset = node["angleOffset"].AsDouble;
                    if (!CheckIsNull(node["ease"]))
                        movetrack.ease = node["ease"].ToString().TrimLR().Parse<Ease>();
                    if (!CheckIsNull(node["maxVfxOnly"]))
                        movetrack.maxVfxOnly = node["maxVfxOnly"].ToString().TrimLR().Parse<Toggle>();
                    if (!CheckIsNull(node["eventTag"]))
                        movetrack.eventTag = node["eventTag"].ToString().TrimLR();
                    if (!CheckIsNull(node["active"]))
                        movetrack.active = node["active"].AsBool;
                    return movetrack;
                case LevelEventType.MultiPlanet:
                    MultiPlanet multiplanet = new MultiPlanet();
                    if (!CheckIsNull(node["planets"]))
                        multiplanet.planets = node["planets"].AsInt;
                    if (!CheckIsNull(node["active"]))
                        multiplanet.active = node["active"].AsBool;
                    return multiplanet;
                case LevelEventType.Twirl:
                    Twirl twirl = new Twirl();
                    if (!CheckIsNull(node["active"]))
                        twirl.active = node["active"].AsBool;
                    return twirl;
                case LevelEventType.Pause:
                    Pause pause = new Pause();
                    if (!CheckIsNull(node["duration"]))
                        pause.duration = node["duration"].AsDouble;
                    if (!CheckIsNull(node["countdownTicks"]))
                        pause.countdownTicks = node["countdownTicks"].AsInt;
                    if (!CheckIsNull(node["angleCorrectionDir"]))
                        pause.angleCorrectionDir = node["angleCorrectionDir"].AsDouble;
                    if (!CheckIsNull(node["active"]))
                        pause.active = node["active"].AsBool;
                    return pause;
                case LevelEventType.PlaySound:
                    PlaySound playsound = new PlaySound();
                    if (!CheckIsNull(node["hitsound"]))
                        playsound.hitsound = node["hitsound"].ToString().TrimLR().Parse<HitSound>();
                    if (!CheckIsNull(node["hitsoundVolume"]))
                        playsound.hitsoundVolume = node["hitsoundVolume"].AsInt;
                    if (!CheckIsNull(node["angleOffset"]))
                        playsound.angleOffset = node["angleOffset"].AsDouble;
                    if (!CheckIsNull(node["eventTag"]))
                        playsound.eventTag = node["eventTag"].ToString().TrimLR();
                    if (!CheckIsNull(node["active"]))
                        playsound.active = node["active"].AsBool;
                    return playsound;
                case LevelEventType.PositionTrack:
                    PositionTrack positiontrack = new PositionTrack();
                    if (!CheckIsNull(node["positionOffset"]))
                        positiontrack.positionOffset = Vector2.FromNode(node["positionOffset"]);
                    if (!CheckIsNull(node["relativeTo"]))
                    {
                        JsonArray relativeto3Arr = node["relativeTo"].AsArray;
                        (int, TileRelativeTo) relativeto3 = (relativeto3Arr[0].AsInt, relativeto3Arr[1].ToString().TrimLR().Parse<TileRelativeTo>());
                        positiontrack.relativeTo = relativeto3;
                    }
                    if (!CheckIsNull(node["rotation"]))
                        positiontrack.rotation = node["rotation"].AsDouble;
                    if (!CheckIsNull(node["scale"]))
                        positiontrack.scale = node["scale"].AsDouble;
                    if (!CheckIsNull(node["opacity"]))
                        positiontrack.opacity = node["opacity"].AsInt;
                    if (!CheckIsNull(node["justThisTile"]))
                        positiontrack.justThisTile = node["justThisTile"].ToString().TrimLR().Parse<Toggle>();
                    if (!CheckIsNull(node["editorOnly"]))
                        positiontrack.editorOnly = node["editorOnly"].ToString().TrimLR().Parse<Toggle>();
                    if (!CheckIsNull(node["active"]))
                        positiontrack.active = node["active"].AsBool;
                    return positiontrack;
                case LevelEventType.RecolorTrack:
                    RecolorTrack recolortrack = new RecolorTrack();
                    if (!CheckIsNull(node["startTile"]))
                    {
                        JsonArray starttile4Arr = node["startTile"].AsArray;
                        (int, TileRelativeTo) starttile4 = (starttile4Arr[0].AsInt, starttile4Arr[1].ToString().TrimLR().Parse<TileRelativeTo>());
                        recolortrack.startTile = starttile4;
                    }
                    if (!CheckIsNull(node["endTile"]))
                    {
                        JsonArray endtile5Arr = node["endTile"].AsArray;
                        (int, TileRelativeTo) endtile5 = (endtile5Arr[0].AsInt, endtile5Arr[1].ToString().TrimLR().Parse<TileRelativeTo>());
                        recolortrack.endTile = endtile5;
                    }
                    if (!CheckIsNull(node["trackColorType"]))
                        recolortrack.trackColorType = node["trackColorType"].ToString().TrimLR().Parse<TrackColorType>();
                    if (!CheckIsNull(node["trackColor"]))
                        recolortrack.trackColor = node["trackColor"].ToString().TrimLR();
                    if (!CheckIsNull(node["secondaryTrackColor"]))
                        recolortrack.secondaryTrackColor = node["secondaryTrackColor"].ToString().TrimLR();
                    if (!CheckIsNull(node["trackColorAnimDuration"]))
                        recolortrack.trackColorAnimDuration = node["trackColorAnimDuration"].AsDouble;
                    if (!CheckIsNull(node["trackColorPulse"]))
                        recolortrack.trackColorPulse = node["trackColorPulse"].ToString().TrimLR().Parse<TrackColorPulse>();
                    if (!CheckIsNull(node["trackPulseLength"]))
                        recolortrack.trackPulseLength = node["trackPulseLength"].AsDouble;
                    if (!CheckIsNull(node["trackStyle"]))
                        recolortrack.trackStyle = node["trackStyle"].ToString().TrimLR().Parse<TrackStyle>();
                    if (!CheckIsNull(node["angleOffset"]))
                        recolortrack.angleOffset = node["angleOffset"].AsDouble;
                    if (!CheckIsNull(node["eventTag"]))
                        recolortrack.eventTag = node["eventTag"].ToString().TrimLR();
                    if (!CheckIsNull(node["active"]))
                        recolortrack.active = node["active"].AsBool;
                    return recolortrack;
                case LevelEventType.RepeatEvents:
                    RepeatEvents repeatevents = new RepeatEvents();
                    if (!CheckIsNull(node["repetitions"]))
                        repeatevents.repetitions = node["repetitions"].AsInt;
                    if (!CheckIsNull(node["interval"]))
                        repeatevents.interval = node["interval"].AsDouble;
                    if (!CheckIsNull(node["tag"]))
                        repeatevents.tag = node["tag"].ToString().TrimLR();
                    if (!CheckIsNull(node["active"]))
                        repeatevents.active = node["active"].AsBool;
                    return repeatevents;
                case LevelEventType.ScaleMargin:
                    ScaleMargin scalemargin = new ScaleMargin();
                    if (!CheckIsNull(node["scale"]))
                        scalemargin.scale = node["scale"].AsInt;
                    if (!CheckIsNull(node["active"]))
                        scalemargin.active = node["active"].AsBool;
                    return scalemargin;
                case LevelEventType.ScaleRadius:
                    ScaleRadius scaleradius = new ScaleRadius();
                    if (!CheckIsNull(node["scale"]))
                        scaleradius.scale = node["scale"].AsInt;
                    if (!CheckIsNull(node["active"]))
                        scaleradius.active = node["active"].AsBool;
                    return scaleradius;
                case LevelEventType.ScreenScroll:
                    ScreenScroll screenscroll = new ScreenScroll();
                    if (!CheckIsNull(node["scroll"]))
                        screenscroll.scroll = Vector2.FromNode(node["scroll"]);
                    if (!CheckIsNull(node["angleOffset"]))
                        screenscroll.angleOffset = node["angleOffset"].AsDouble;
                    if (!CheckIsNull(node["eventTag"]))
                        screenscroll.eventTag = node["eventTag"].ToString().TrimLR();
                    if (!CheckIsNull(node["active"]))
                        screenscroll.active = node["active"].AsBool;
                    return screenscroll;
                case LevelEventType.ScreenTile:
                    ScreenTile screentile = new ScreenTile();
                    if (!CheckIsNull(node["tile"]))
                        screentile.tile = Vector2.FromNode(node["tile"]);
                    if (!CheckIsNull(node["angleOffset"]))
                        screentile.angleOffset = node["angleOffset"].AsDouble;
                    if (!CheckIsNull(node["eventTag"]))
                        screentile.eventTag = node["eventTag"].ToString().TrimLR();
                    if (!CheckIsNull(node["active"]))
                        screentile.active = node["active"].AsBool;
                    return screentile;
                case LevelEventType.SetConditionalEvents:
                    SetConditionalEvents setconditionalevents = new SetConditionalEvents();
                    if (!CheckIsNull(node["perfectTag"]))
                        setconditionalevents.perfectTag = node["perfectTag"].ToString().TrimLR();
                    if (!CheckIsNull(node["hitTag"]))
                        setconditionalevents.hitTag = node["hitTag"].ToString().TrimLR();
                    if (!CheckIsNull(node["barelyTag"]))
                        setconditionalevents.barelyTag = node["barelyTag"].ToString().TrimLR();
                    if (!CheckIsNull(node["missTag"]))
                        setconditionalevents.missTag = node["missTag"].ToString().TrimLR();
                    if (!CheckIsNull(node["lossTag"]))
                        setconditionalevents.lossTag = node["lossTag"].ToString().TrimLR();
                    if (!CheckIsNull(node["active"]))
                        setconditionalevents.active = node["active"].AsBool;
                    return setconditionalevents;
                case LevelEventType.SetFilter:
                    SetFilter setfilter = new SetFilter();
                    if (!CheckIsNull(node["filter"]))
                        setfilter.filter = node["filter"].ToString().TrimLR().Parse<Filter>();
                    if (!CheckIsNull(node["enabled"]))
                        setfilter.enabled = node["enabled"].ToString().TrimLR().Parse<Toggle>();
                    if (!CheckIsNull(node["intensity"]))
                        setfilter.intensity = node["intensity"].AsDouble;
                    if (!CheckIsNull(node["duration"]))
                        setfilter.duration = node["duration"].AsDouble;
                    if (!CheckIsNull(node["ease"]))
                        setfilter.ease = node["ease"].ToString().TrimLR().Parse<Ease>();
                    if (!CheckIsNull(node["disableOthers"]))
                        setfilter.disableOthers = node["disableOthers"].ToString().TrimLR().Parse<Toggle>();
                    if (!CheckIsNull(node["angleOffset"]))
                        setfilter.angleOffset = node["angleOffset"].AsDouble;
                    if (!CheckIsNull(node["eventTag"]))
                        setfilter.eventTag = node["eventTag"].ToString().TrimLR();
                    if (!CheckIsNull(node["active"]))
                        setfilter.active = node["active"].AsBool;
                    return setfilter;
                case LevelEventType.SetHitsound:
                    SetHitsound sethitsound = new SetHitsound();
                    if (!CheckIsNull(node["gameSound"]))
                        sethitsound.gameSound = node["gameSound"].ToString().TrimLR().Parse<GameSound>();
                    if (!CheckIsNull(node["hitsound"]))
                        sethitsound.hitsound = node["hitsound"].ToString().TrimLR().Parse<HitSound>();
                    if (!CheckIsNull(node["hitsoundVolume"]))
                        sethitsound.hitsoundVolume = node["hitsoundVolume"].AsInt;
                    if (!CheckIsNull(node["active"]))
                        sethitsound.active = node["active"].AsBool;
                    return sethitsound;
                case LevelEventType.SetHoldSound:
                    SetHoldSound setholdsound = new SetHoldSound();
                    if (!CheckIsNull(node["holdStartSound"]))
                        setholdsound.holdStartSound = node["holdStartSound"].ToString().TrimLR().Parse<HoldStartSound>();
                    if (!CheckIsNull(node["holdLoopSound"]))
                        setholdsound.holdLoopSound = node["holdLoopSound"].ToString().TrimLR().Parse<HoldLoopSound>();
                    if (!CheckIsNull(node["holdEndSound"]))
                        setholdsound.holdEndSound = node["holdEndSound"].ToString().TrimLR().Parse<HoldEndSound>();
                    if (!CheckIsNull(node["holdMidSound"]))
                        setholdsound.holdMidSound = node["holdMidSound"].ToString().TrimLR().Parse<HoldMidSound>();
                    if (!CheckIsNull(node["holdMidSoundType"]))
                        setholdsound.holdMidSoundType = node["holdMidSoundType"].ToString().TrimLR().Parse<HoldMidSoundType>();
                    if (!CheckIsNull(node["holdMidSoundDelay"]))
                        setholdsound.holdMidSoundDelay = node["holdMidSoundDelay"].AsDouble;
                    if (!CheckIsNull(node["holdMidSoundTimingRelativeTo"]))
                        setholdsound.holdMidSoundTimingRelativeTo = node["holdMidSoundTimingRelativeTo"].ToString().TrimLR().Parse<HoldMidSoundTimingRelativeTo>();
                    if (!CheckIsNull(node["holdSoundVolume"]))
                        setholdsound.holdSoundVolume = node["holdSoundVolume"].AsInt;
                    if (!CheckIsNull(node["active"]))
                        setholdsound.active = node["active"].AsBool;
                    return setholdsound;
                case LevelEventType.SetPlanetRotation:
                    SetPlanetRotation setplanetrotation = new SetPlanetRotation();
                    if (!CheckIsNull(node["ease"]))
                        setplanetrotation.ease = node["ease"].ToString().TrimLR().Parse<Ease>();
                    if (!CheckIsNull(node["easeParts"]))
                        setplanetrotation.easeParts = node["easeParts"].AsInt;
                    if (!CheckIsNull(node["easePartBehavior"]))
                        setplanetrotation.easePartBehavior = node["easePartBehavior"].ToString().TrimLR().Parse<EasePartBehavior>();
                    if (!CheckIsNull(node["active"]))
                        setplanetrotation.active = node["active"].AsBool;
                    return setplanetrotation;
                case LevelEventType.SetSpeed:
                    SetSpeed setspeed = new SetSpeed();
                    if (!CheckIsNull(node["speedType"]))
                        setspeed.speedType = node["speedType"].ToString().TrimLR().Parse<SpeedType>();
                    if (!CheckIsNull(node["beatsPerMinute"]))
                        setspeed.beatsPerMinute = node["beatsPerMinute"].AsDouble;
                    if (!CheckIsNull(node["bpmMultiplier"]))
                        setspeed.bpmMultiplier = node["bpmMultiplier"].AsDouble;
                    if (!CheckIsNull(node["active"]))
                        setspeed.active = node["active"].AsBool;
                    return setspeed;
                case LevelEventType.SetText:
                    SetText settext = new SetText();
                    if (!CheckIsNull(node["decText"]))
                        settext.decText = node["decText"].ToString().TrimLR();
                    if (!CheckIsNull(node["tag"]))
                        settext.tag = node["tag"].ToString().TrimLR();
                    if (!CheckIsNull(node["angleOffset"]))
                        settext.angleOffset = node["angleOffset"].AsDouble;
                    if (!CheckIsNull(node["eventTag"]))
                        settext.eventTag = node["eventTag"].ToString().TrimLR();
                    if (!CheckIsNull(node["active"]))
                        settext.active = node["active"].AsBool;
                    return settext;
                case LevelEventType.ShakeScreen:
                    ShakeScreen shakescreen = new ShakeScreen();
                    if (!CheckIsNull(node["duration"]))
                        shakescreen.duration = node["duration"].AsDouble;
                    if (!CheckIsNull(node["strength"]))
                        shakescreen.strength = node["strength"].AsDouble;
                    if (!CheckIsNull(node["intensity"]))
                        shakescreen.intensity = node["intensity"].AsDouble;
                    if (!CheckIsNull(node["fadeOut"]))
                        shakescreen.fadeOut = node["fadeOut"].ToString().TrimLR().Parse<Toggle>();
                    if (!CheckIsNull(node["angleOffset"]))
                        shakescreen.angleOffset = node["angleOffset"].AsDouble;
                    if (!CheckIsNull(node["eventTag"]))
                        shakescreen.eventTag = node["eventTag"].ToString().TrimLR();
                    if (!CheckIsNull(node["active"]))
                        shakescreen.active = node["active"].AsBool;
                    return shakescreen;
                default: return new UnknownAction(node);
            }
        }
    }
}
