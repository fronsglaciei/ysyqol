using Plot;
using PlotDesigher;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FG.Mods.YSYard.QoL.Models.Simulations;

/// <summary>
/// Model of PlotWindow's updates without rendering
/// </summary>
public class LevelSimulation
{
    private readonly Dictionary<int, ILevelSimulationLog> _log = [];

	private readonly SimulatedCharacterControl _chA = new();

	private readonly SimulatedCharacterControl _chB = new();

	private readonly SimulatedCharacterControl _chC = new();

    //private readonly List<PlotReviewData> _plotReviewData = [];
    private readonly List<bmj> _plotReviewData = [];

    private bool _textOffsetChanged = false;

    private Vector2 _conversationTextOffset = Vector2.zero;

    private Vector2 _nameTextOffset = Vector2.zero;

    private bool _dialogVisible = true;

    private readonly SimulatedImageControl _bg = new();

    private readonly SimulatedImageControl[] _cgs
        = [new(), new(), new(), new(), new()];

    private readonly SimulatedImageControl _cgbg = new();

	private readonly SimulatedImageControl _sysd = new();

    private readonly SimulatedImageControl[] _cartoons
        = [new(), new(), new(), new(), new(), new()];

    private readonly SimulatedImageControl _screenFade = new();

    private readonly SimulatedImageControl _bgMask = new();

    private int _bgmId;

    //private FMODManager.EHandleCurBGM _bgmState;
    private cd.bv _bgmState;

    private Say _replaySay;

    private Say _lastPlayedSay;

    private OptionalTask _replayOptionalTask;

    internal bool TryGetLog(
        int taskId, [MaybeNullWhen(false)] out ILevelSimulationLog log)
        => this._log.TryGetValue(taskId, out log);

    internal void SetLog(ILevelSimulationLog log)
        => this._log[log.TaskId] = log;

    internal void ApplySimulationTo(
        //LevelDesignerManager ldm)
        bmo ldm)
    {
        //var pw = ldm.mPlotWindow;
        var pw = ldm.bhls;
        if (pw is null)
        {
            return;
        }
        if (!this.Validate())
        {
            return;
        }

        //this._bg.ApplyStateTo(pw.mBGControl);
        this._bg.ApplyStateTo(pw.befg);
        //this._cgbg.ApplyStateTo(pw.mCGBGControl);
        this._cgbg.ApplyStateTo(pw.befh);

        //this._bgMask.ApplyStateTo(pw.mBGMaskControl);
        this._bgMask.ApplyStateTo(pw.befi);

        //var dstCGs = pw.mCGImage;
        var dstCGs = pw.bedb;
        if (dstCGs is not null)
        {
            var cnt = Math.Min(this._cgs.Length, dstCGs.Count);
            for (var i = 0; i < cnt; i++)
            {
                this._cgs[i].ApplyStateTo(dstCGs[i], pw);
            }
        }

        //var dstCartoons = pw.mCartoonImage;
        var dstCartoons = pw.bedc;
        if (dstCartoons is not null)
        {
            var cnt = Math.Min(this._cartoons.Length, dstCartoons.Count);
            for (var i = 0; i< cnt; i++)
            {
                this._cartoons[i].ApplyStateTo(dstCartoons[i], pw);
            }
        }

        //var dstCharas = pw.CharacterDic;
        var dstCharas = pw.befb;
        if (dstCharas is not null)
        {
            if (dstCharas.TryGetValue(0, out var pccA))
            {
                this._chA.ApplyStateTo(pccA);
            }
            if (dstCharas.TryGetValue(1, out var pccB))
            {
                this._chB.ApplyStateTo(pccB);
            }
            if (dstCharas.TryGetValue(2, out var pccC))
            {
                this._chC.ApplyStateTo(pccC);
            }
            if (pccA is not null
                && pccB is not null
                && pccC is not null)
            {
                // force reordering
                //var pccs = new List<PlotCharacterControl> { pccA, pccB, pccC };
                var pccs = new List<bie> { pccA, pccB, pccC };
                //pccs.Sort((a, b) => a.order.CompareTo(b.order));
                pccs.Sort((a, b) => a.bdcu.CompareTo(b.bdcu));
                foreach (var pcc in pccs)
                {
                    //pcc.SetLastOrder();
                    pcc.luv();
                }
                //pw.currentOrder = 3;
                pw.befr = 3;
            }
        }

        //if ((pw.SayDialogs?.TryGetValue(0, out var dialog) ?? false)
        //    && dialog is not null)
        if ((pw.befl?.TryGetValue(0, out var dialog) ?? false)
            && dialog is not null)
        {
            if (this._textOffsetChanged)
            {
                //dialog.SetConversationPos(this._conversationTextOffset);
                dialog.ltc(this._conversationTextOffset);
                //dialog.SetCharacterPos(this._nameTextOffset);
                dialog.ltb(this._nameTextOffset);
            }

            //pw.ShowOrHideDialog(DialogStyle.Normal, this._dialogVisible);
            pw.nlc(bmd.Normal, this._dialogVisible);
        }

        //this._screenFade.ApplyStateTo(pw.mScreenFadeTexture);
        this._screenFade.ApplyStateTo(pw.bedt);

        //if (ldm.ReviewData is not null)
        if (ldm.bean is not null)
        {
            //ldm.ReviewData.Clear();
            ldm.bean.Clear();
            foreach (var pld in this._plotReviewData)
            {
                //ldm.ReviewData.Add(pld);
                ldm.bean.Add(pld);
            }
        }

        //if (this._bgmState == FMODManager.EHandleCurBGM.None)
        if (this._bgmState == cd.bv.None)
        {
            //FMODManager.FadeSwitchStoryBGM(
            cd.cqj(
                new(this._bgmId, string.Empty, 0), 1f, 1f, null);
        }

        if (this._replaySay is not null)
        {
            //this._replaySay.OnStart();
            this._replaySay.lzd();
        }
        else if (this._replayOptionalTask is not null)
        {
            if (this._lastPlayedSay is not null)
            {
                this.ForcePlaySay(ldm, this._lastPlayedSay);
            }
            else
            {
                //pw.ShowOrHideDialog(DialogStyle.Normal, this._dialogVisible);
                pw.nlc(bmd.Normal, false);
            }
            //this._replayOptionalTask.OnStart();
            this._replayOptionalTask.lzd();
        }
    }

    internal void HideAllCartoons()
    {
        foreach (var ct in this._cartoons)
        {
            ct.Visible = false;
        }
    }

    internal void SetImageVisible(
        //CharacterEnum imageType, bool visible)
        blu imageType, bool visible)
        => this.GetImageControl(imageType)?.Visible = visible;

    internal void SetImagePosition(
        //CharacterEnum imageType, Vector3 position)
        blu imageType, Vector3 position)
        => this.GetTransformAccessibleControl(imageType)?.Position = position;

    internal void SetImageScale(
        //CharacterEnum imageType, Vector3 scale)
        blu imageType, Vector3 scale)
        => this.GetTransformAccessibleControl(imageType)?.Scale = scale;

    internal void SetImageResource(
        //CharacterEnum imageType, string resourceName)
        blu imageType, string resourceName)
        => this.GetImageControl(imageType)?.ResourceName = resourceName;

    internal void SetImageColor(
        //CharacterEnum imageType, UnityEngine.Color color)
        blu imageType, UnityEngine.Color color)
    {
        var sic = this.GetImageControl(imageType);
        if (sic is not null)
        {
            sic.Color = color;
            sic.ColorChanged = true;
        }
    }

    internal void SetImageFlipped(
        //CharacterEnum imageType, bool flipped)
        blu imageType, bool flipped)
        => this.GetImageControl(imageType)?.Flipped = flipped;

    internal void SetImageAutoResize(
        //CharacterEnum imageType, bool autoResize)
        blu imageType, bool autoResize)
        => this.GetAutoResizableControl(imageType)?.AutoResize = autoResize;

    internal void SetSayCharacters(bool showA, bool showB, bool showC)
    {
        var pairs = new[]
        {
            (this._chA, showA),
            (this._chB, showB),
            (this._chC, showC),
        };
        var ctrls = new LinkedList<SimulatedCharacterControl>();
        foreach (var (ctrl, show) in pairs)
        {
            if (show)
            {
                ctrls.AddLast(ctrl);
                ctrl.Darkened = false;
            }
            else
            {
                ctrls.AddFirst(ctrl);
                ctrl.Darkened = true;
            }
        }
        foreach (var (ctrl, order) in ctrls.Select((x, i) => (x, i)))
        {
            ctrl.Order = order;
        }
    }

    internal void SetTextData(bmj review) // (PlotReviewData review)
    {
        //if (review.conversationID != 0
        //    && string.IsNullOrEmpty(review.conversation))
        if (review.bdzp != 0
            && string.IsNullOrEmpty(review.bdzr))
        {
            //review.conversation =
            //    GameAPI.GetConvesationStr(review.conversationID);
            review.bdzr = cy.cun(review.bdzp);
        }
        this._plotReviewData.Add(review);
    }

    internal void SetTextOffset(Vector2 conversationOffset, Vector2 nameOffset)
    {
        this._textOffsetChanged = true;
        this._conversationTextOffset = conversationOffset;
        this._nameTextOffset = nameOffset;
    }

    internal void SetDialogVisible(bool visible)
        => this._dialogVisible = visible;

    internal void ShowCartoon(
        //CharacterEnum imageType, Vector3 position, string resourceName)
        blu imageType, Vector3 position, string resourceName)
    {
        var sic = this.GetCartoonControl(imageType);
        if (sic is not null)
        {
            sic.Visible = true;
            sic.Position = position;
            sic.ResourceName = resourceName;
        }
    }

    internal void SetBackground(bool show, string resourceName)
    {
        this._bg.Visible = show;
        if (show)
        {
            this._bg.ResourceName = resourceName;
        }
    }

    internal void SetCGBackground(bool show, string resourceName)
    {
        this._cgbg.Visible = show;
        if (show)
        {
            this._cgbg.ResourceName = resourceName;
        }
    }

    internal void SetScreenFade(bool show, UnityEngine.Color color)
    {
        this._screenFade.Visible = show;
        this._screenFade.Color = color;
        this._screenFade.ColorChanged = true;
    }

    internal void SetBackgroundMask(bool show, string resourceName, UnityEngine.Color color)
    {
        this._bgMask.Visible = show;
        if (show)
        {
            this._bgMask.ResourceName = resourceName;
            this._bgMask.Color = color;
            this._bgMask.ColorChanged = true;
        }
        else
        {
            this._bgMask.ResourceName = string.Empty;
        }
    }

    internal void SetCharacterExpressionColor(
        //CharacterEnum imageType, UnityEngine.Color expColor)
        blu imageType, UnityEngine.Color expColor)
        => this.GetCharacterControl(imageType)?.ExpressionColor = expColor;

    internal void SetCharacterBlockColor(
        //CharacterEnum imageType, UnityEngine.Color blkColor)
        blu imageType, UnityEngine.Color blkColor)
        => this.GetCharacterControl(imageType)?.BlockColor = blkColor;

    internal void PlayBGM(int id)
    {
        this._bgmId = id;
        //this._bgmState = FMODManager.EHandleCurBGM.None;
        this._bgmState = cd.bv.None;
    }

    internal void PauseBGM()
    {
        //this._bgmState = FMODManager.EHandleCurBGM.Pause;
        this._bgmState = cd.bv.Pause;
    }

    internal void CleanCharacters()
    {
        foreach (var ctrl in new[] { this._chA, this._chB, this._chC })
        {
            ctrl.Visible = false;
            ctrl.ExpressionColor = UnityEngine.Color.white;
            ctrl.BlockColor = UnityEngine.Color.gray;
        }
    }

    internal void CleanCartoons()
    {
        foreach (var ct in this._cartoons)
        {
            ct.Visible = false;
            ct.ResourceName = string.Empty;
            ct.Scale = new Vector3(0.7f, 0.7f, 1f);
        }
    }

    internal void SetReplaySay(Say say) => this._replaySay = say;

    internal void SetLastPlayedSay(Say say) => this._lastPlayedSay = say;

    internal void SetReplayOptionalTask(OptionalTask ot) => this._replayOptionalTask = ot;

    private bool Validate()
    {
        if (this._bg.Visible && this._cgbg.Visible)
        {
            Plugin.Log.LogError($"[{nameof(LevelSimulation)}]: BG and CGBG cannot coexist");
            return false;
        }

        return true;
    }

    private SimulatedImageControl GetImageControl(
        //CharacterEnum imageType)
        blu imageType)
        => imageType switch
        {
            blu.CharacterA => this._chA,
            blu.CharacterB => this._chB,
            blu.CharacterC => this._chC,
            blu.Background => this._bg,
            blu.CGImage1 => this._cgs[0],
            blu.CGImage2 => this._cgs[1],
            blu.CGImage3 => this._cgs[2],
            blu.CGImage4 => this._cgs[3],
            blu.CGImage5 => this._cgs[4],
            blu.CGBackground => this._cgbg,
            blu.SystemDialog => this._sysd,
            blu.CartoonImage1 => this._cartoons[0],
            blu.CartoonImage2 => this._cartoons[1],
            blu.CartoonImage3 => this._cartoons[2],
            blu.CartoonImage4 => this._cartoons[3],
            blu.CartoonImage5 => this._cartoons[4],
            blu.CartoonImage6 => this._cartoons[5],
            _ => null
        };

    private SimulatedImageControl GetTransformAccessibleControl(
        //CharacterEnum imageType)
        blu imageType)
        => imageType switch
        {
            blu.CharacterA => this._chA,
            blu.CharacterB => this._chB,
            blu.CharacterC => this._chC,
            blu.Background => this._bg,
            blu.CGImage1 => this._cgs[0],
            blu.CGImage2 => this._cgs[1],
            blu.CGImage3 => this._cgs[2],
            blu.CGImage4 => this._cgs[3],
            blu.CGImage5 => this._cgs[4],
            blu.CGBackground => this._cgbg,
            blu.SystemDialog => null,
            blu.CartoonImage1 => this._cartoons[0],
            blu.CartoonImage2 => this._cartoons[1],
            blu.CartoonImage3 => this._cartoons[2],
            blu.CartoonImage4 => this._cartoons[3],
            blu.CartoonImage5 => this._cartoons[4],
            blu.CartoonImage6 => this._cartoons[5],
            _ => null
        };

    private SimulatedImageControl GetAutoResizableControl(
        //CharacterEnum imageType)
        blu imageType)
        => imageType switch
        {
            blu.CharacterA => this._chA,
            blu.CharacterB => this._chB,
            blu.CharacterC => this._chC,
            blu.CGImage1 => this._cgs[0],
            blu.CGImage2 => this._cgs[1],
            blu.CGImage3 => this._cgs[2],
            blu.CGImage4 => this._cgs[3],
            blu.CGImage5 => this._cgs[4],
            _ => null
        };

    private SimulatedCharacterControl GetCharacterControl(
        //CharacterEnum imageType)
        blu imageType)
        => imageType switch
        {
            blu.CharacterA => this._chA,
            blu.CharacterB => this._chB,
            blu.CharacterC => this._chC,
            _ => null,
        };

    private SimulatedImageControl GetCartoonControl(
        //CharacterEnum imageType)
        blu imageType)
        => imageType switch
        {
            blu.CartoonImage1 => this._cartoons[0],
            blu.CartoonImage2 => this._cartoons[1],
            blu.CartoonImage3 => this._cartoons[2],
            blu.CartoonImage4 => this._cartoons[3],
            blu.CartoonImage5 => this._cartoons[4],
            blu.CartoonImage6 => this._cartoons[5],
            _ => null
        };

    private void ForcePlaySay(
        //LevelDesignerManager ldm, Say say)
        bmo ldm, Say say)
    {
        if (ldm is null || say is null
            //|| say.Data is null
            || say.bhlf is null)
        {
            return;
        }

        //for (var i = ldm.ReviewData.Count - 1;
        var foundIdx = -1;
        for (var i = ldm.bean.Count - 1;
            -1 < i; i--)
        {
            var prd = ldm.bean[i];
            //if (prd?.conversationID == say.Data.conversationID)
            if (prd?.bdzp == say.bhlf.bdry)
            {
                foundIdx = i;
                break;
            }
        }
        if (-1 < foundIdx)
        {
            ldm.bean.RemoveAt(foundIdx);
        }

        //ldm.mPlotWindow.onSpeedUpDown();
        ldm.bhls.njp();

        //say.OnStart();
        say.lzd();

        //ldm.mPlotWindow.onSpeedUpUp();
        ldm.bhls.njq();
    }

    private class SimulatedImageControl
    {
        internal bool Visible { get; set; }

        internal Vector3 Position { get; set; }

        internal Vector3 Scale { get; set; } = Vector3.one;

        internal string ResourceName { get; set; }

        internal UnityEngine.Color Color { get; set; }

        internal bool ColorChanged { get; set; }

        internal bool Flipped { get; set; }

        internal bool AutoResize { get; set; }

        internal void ApplyStateTo(
            //ImageControl imgCtrl)
            bic imgCtrl)
        {
            if (imgCtrl is null)
            {
                return;
            }

            //imgCtrl.SetImage(
            //    this.ResourceName,
            //    string.IsNullOrEmpty(this.ResourceName));
            imgCtrl.lud(
                this.ResourceName,
                string.IsNullOrEmpty(this.ResourceName));
            //imgCtrl.SetActiveSafely(this.Visible);
            imgCtrl.lui(this.Visible);
            //imgCtrl.SetPostion(this.Position);
            imgCtrl.lub(this.Position);
            if (this.ColorChanged)
            {
                //imgCtrl.SetColor(this.Color);
                imgCtrl.luc(this.Color);
            }
        }

        internal void ApplyStateTo(Image img, PlotWindow srcPlotWindow)
        {
            if (img is null || srcPlotWindow is null)
            {
                return;
            }

            //var sprite = SpriteAtlasManager.GetSpriteByName(this.ResourceName);
            var sprite = cn.ctr(this.ResourceName);
            if (sprite is not null)
            {
                img.sprite = sprite;
                //img.gameObject.SetActiveSafely(true);
                img.gameObject.cvk(true);
            }
            img.transform.localPosition = this.Position;
            img.SetNativeSize();
            if (this.AutoResize)
            {
                var rtImg = img.rectTransform;
                //var yDelta = srcPlotWindow.width * rtImg.sizeDelta.y / rtImg.sizeDelta.x;
                var yDelta = srcPlotWindow.bege * rtImg.sizeDelta.y / rtImg.sizeDelta.x;
                //rtImg.sizeDelta = new(srcPlotWindow.width, yDelta);
                rtImg.sizeDelta = new(srcPlotWindow.bege, yDelta);
            }
            img.transform.localScale = new(
                this.Flipped ? -this.Scale.x : this.Scale.x,
                this.Scale.y, this.Scale.z);
            if (this.ColorChanged)
            {
                img.color = this.Color;
            }

            if (!this.Visible)
            {
                //img.gameObject.SetActiveSafely(false);
                img.gameObject.cvk(false);
            }
        }

        internal void ApplyStateTo(RawImage rawImg)
        {
            if (rawImg is null)
            {
                return;
            }

            //rawImg.gameObject.SetActiveSafely(true);
            rawImg.gameObject.cvk(true);

            if (this.ColorChanged)
            {
                rawImg.color = this.Color;
            }

            //rawImg.gameObject.SetActiveSafely(this.Visible);
            rawImg.gameObject.cvk(this.Visible);
        }
    }

    private class SimulatedCharacterControl : SimulatedImageControl
    {
        internal int Order { get; set; }

        internal bool Darkened { get; set; }

        internal UnityEngine.Color ExpressionColor { get; set; }

        internal UnityEngine.Color BlockColor { get; set; }

        internal void ApplyStateTo(
            //PlotCharacterControl pcc)
            bie pcc)
        {
            if (pcc is null)
            {
                return;
            }

            //pcc.SetActive(true);
            pcc.lus(true);

            //pcc.SetPostion(this.Position);
            pcc.lur(this.Position);
            //pcc.gameObject.transform.localScale = this.Scale;
            pcc.bdco.transform.localScale = this.Scale;
            if (int.TryParse(this.ResourceName, out var tmpHeroId))
            {
                //pcc.heroID = tmpHeroId;
                pcc.bdcr = tmpHeroId;
                //var exprAnim = ExpressionAnimatorManager.Instance
                //    .AddExpressionAnimByHeroID(
                //        tmpHeroId, pcc.gameObject.transform, nameof(PlotWindow));
                var exprAnim = dx.bgsy.dci(
                    tmpHeroId, pcc.bdco.transform, nameof(PlotWindow));
                if (exprAnim is not null)
                {
                    //pcc.expressionAnimator = exprAnim;
                    pcc.bdct = exprAnim;
                    if (this.Flipped && tmpHeroId != 0)
                    {
                        //pcc.expressionAnimator.transform.localScale = new(-1f, 1f, 1f);
                        pcc.bdct.transform.localScale = new(-1f, 1f, 1f);
                    }
                    if (this.ColorChanged)
                    {
                        //foreach (var img in pcc.expressionAnimator.GetAllImage())
                        foreach (var img in pcc.bdct.ecl())
                        {
                            img.color = this.Color;
                        }
                    }
                }
            }
            //pcc.SetSize(this.AutoResize, Screen.width);
            pcc.luw(this.AutoResize, Screen.width);

            //pcc.order = this.Order;
            pcc.bdcu = this.Order;
            //pcc.SetCharatorColor(this.ExpressionColor);
            pcc.luo(this.ExpressionColor);
            //pcc.SetBlockColor(this.BlockColor);
            pcc.lup(this.BlockColor);
            //pcc.SetDark(this.Darkened);
            pcc.luq(this.Darkened);

            //pcc.SetActive(this.Visible);
            pcc.lus(this.Visible);
        }
    }
}
