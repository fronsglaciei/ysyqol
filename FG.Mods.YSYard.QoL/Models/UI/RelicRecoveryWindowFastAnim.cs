using DG.Tweening;
using FG.Mods.YSYard.QoL.Services;
using Foundation.UI;
using HotelModule.UI;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace FG.Mods.YSYard.QoL.Models.UI;

internal class RelicRecoveryWindowFastAnim
{
    private const string NODE_MATERIAL_PROPERTY
        = "Vector1_019f4df431be439b861e1b306d106ee8";

    private readonly vc _rrw; // private readonly RelicRecoveryWindow _rrw;

    internal RelicRecoveryWindowFastAnim()
    {
        var rrw = ExUIManager
            .GetOrLoad<vc>("UI_RelicRecovery")
            ?? throw new InvalidOperationException(
                "Failed to load UI_RelicRecovery");
        this._rrw = rrw;

        // disable "blood moon" animation
        //foreach (var tw in rrw.mGameObject
        foreach (var tw in rrw.zqg
            .GetComponentsInChildren<DOTweenAnimation>(true))
        {
            tw?.tween?.Kill(true);
        }
        //rrw.mGameObject.transform.Find("StartAnim")?.gameObject.SetActive(false);
        rrw.zqg.transform.Find("StartAnim")?.gameObject.SetActive(false);

        // overwrite button listeners
        //rrw.Start.onClick.RemoveAllListeners();
        //rrw.Start.onClick.AddListener(
        //    (UnityAction)this.DissoluteRelicsNoAnim);
        var onClickStart = rrw.ywd.onClick;
        onClickStart.RemoveAllListeners();
        onClickStart.AddListener((UnityAction)this.DissoluteRelicsNoAnim);
        //rrw.Fail.onClick.RemoveAllListeners();
        //rrw.Fail.onClick.AddListener(
        //    (UnityAction)this.DissoluteRelicsNoAnim);
        var onClickFail = rrw.ywe.onClick;
        onClickFail.RemoveAllListeners();
        onClickFail.AddListener((UnityAction)this.DissoluteRelicsNoAnim);
    }

    internal void Open()
    {
        //if (!this._rrw.IsOpen)
        if (!this._rrw.zqh)
        {
            //this._rrw.Open();
            this._rrw.jto();
        }
    }

    internal void RefreshNoAnim(
        int num, bool isFail, Il2CppSystem.Action callback)
    {
        var rrw = this._rrw;

        //rrw.Num.text = num.ToString();
        rrw.ywc.text = num.ToString();
        //rrw.Fail.gameObject.SetActive(isFail);
        rrw.ywe.gameObject.SetActive(isFail);
        //rrw.Start.gameObject.SetActive(!isFail);
        rrw.ywd.gameObject.SetActive(!isFail);
        //rrw._callback = callback;
        rrw.ywi = callback;
        //rrw.Start.interactable = false;
        rrw.ywd.interactable = false;
        //rrw.Fail.interactable = isFail;
        rrw.ywe.interactable = isFail;

        //var tmpNum = ArtifactManager.Instance.mNormalArtifacts.Count;
        var tmpNum = eg.bgtc.vmi.Count;
        //rrw._currentNum = tmpNum;
        rrw.ywl = tmpNum;
        if (0 < tmpNum)
        {
            //var artifacts = ArtifactManager.Instance.GetArtifactsSortByStar();
            var artifacts = eg.bgtc.dgf();
            for (var i = artifacts.Count - 1; -1 < i; i--)
            {
                var artifact = artifacts[i];
                //if (ItemManager.Instance.GetItem(artifact.ArtifactID) == null
                //    || RelicLevelUpManager.Instance.GetItem(artifact.ArtifactID) == null)
                if (hl.bgvr.GetItem(artifact.vli) == null
                    || ia.bgwg.GetItem(artifact.vli) == null)
                {
                    artifacts.RemoveAt(i);
                    tmpNum--;
                }
            }
            //rrw._currentNum = tmpNum;
            rrw.ywl = tmpNum;
            //rrw.RelicList.GetChild(rrw._currentNum - 1).gameObject.SetActive(true);
            rrw.ywf.GetChild(rrw.ywl - 1).gameObject.SetActive(true);

            //var tListTail = rrw.RelicList.GetChild(tmpNum - 1);
            var tListTail = rrw.ywf.GetChild(tmpNum - 1);
            for (var i = 0; i < tmpNum; i++)
            {
                //if (rrw.RelicNodeList.Count <= i)
                if (rrw.ywj.Count <= i)
                {
                    //rrw.RelicNodeList.Add(
                    //    new(UnityEngine.Object.Instantiate(rrw.Relic)));
                    rrw.ywj.Add(
                        new(UnityEngine.Object.Instantiate(rrw.ywg)));
                }
                //var node = rrw.RelicNodeList[i];
                var node = rrw.ywj[i];
                //rrw.RelicNodeOrderDic[i] = node;
                rrw.ywk[i] = node;
                //node.mTransform.SetParent(tListTail.GetChild(i));
                //node.mTransform.localScale = Vector3.one;
                //node.mTransform.localPosition = Vector3.zero;
                var trans = node.yuk;
                trans.SetParent(tListTail.GetChild(i));
                trans.localScale = Vector3.one;
                trans.localPosition = Vector3.zero;
                //node.SetDisplayArtifact(artifacts[i].ArtifactID, true);
                node.ike(artifacts[i].vli, true);
                //node.mTransform.gameObject.SetActive(true);
                trans.gameObject.SetActive(true);
            }
        }
        //rrw.Start.interactable = true;
        rrw.ywd.interactable = true;
        //rrw.Fail.interactable = true;
        rrw.ywe.interactable = true;
    }

    internal void Close() => this._rrw.Close();

    private void DissoluteRelicsNoAnim()
    {
        var rrw = this._rrw;

        //rrw.Operation.SetActive(false);
        rrw.ywh.SetActive(false);

        //for (var i = rrw._currentNum - 1; -1 < i; i--)
        for (var i = rrw.ywl - 1; -1 < i; i--)
        {
            //if (rrw.RelicNodeOrderDic.TryGetValue(i, out var node))
            if (rrw.ywk.TryGetValue(i, out var node))
            {
                DissoluteNodeNoAnim(node);
            }
        }
        //rrw._callback?.Invoke();
        rrw.ywi?.Invoke();
    }

    private static void DissoluteNodeNoAnim(
        //ArtifactNode node)
        ux node)
    {
        if (node is null)
        {
            throw new ArgumentNullException(nameof(node));
        }

        //node.RelicButton.interactable = false;
        node.yuu.interactable = false;
        //if (node._material.HasProperty(NODE_MATERIAL_PROPERTY))
        if (node.yvb.HasProperty(NODE_MATERIAL_PROPERTY))
        {
            //node._material.SetFloat(NODE_MATERIAL_PROPERTY, 1f);
            node.yvb.SetFloat(NODE_MATERIAL_PROPERTY, 1f);
        }
        //UIHelper.SetGraphicAlpha(node.RelicName, 0f);
        bhg.lrm(node.yuo, 0f);
        //UIHelper.SetGraphicAlpha(node.RelicInfo, 0f);
        bhg.lrm(node.yup, 0f);
    }
}
