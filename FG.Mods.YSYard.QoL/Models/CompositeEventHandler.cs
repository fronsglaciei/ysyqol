using Common;
using System;
using System.Collections.Generic;

namespace FG.Mods.YSYard.QoL.Models;

internal class CompositeEventHandler : IDisposable
{
    private readonly int _eventId;

    private readonly List<Action> _onEvents;

    private readonly Il2CppSystem.Action _onEventRoot;

    internal CompositeEventHandler(
        int eventId, List<Action> onEvents)
    {
        this._eventId = eventId;
        this._onEvents = onEvents ?? [];
        this._onEventRoot = (Il2CppSystem.Action)this.OnEvent;
        //EventManager.RegisterEvent(
        //    this._eventId, this._onEventRoot);
        wr.jdj(this._eventId, this._onEventRoot);
    }

    internal static CompositeEventHandler OnLanguageChanged(
        List<Action> onEvents) => new(27, onEvents);

    private void OnEvent()
    {
        foreach (var onEvent in this._onEvents)
        {
            onEvent?.Invoke();
        }
    }

    public void Dispose() =>
        //EventManager.UnRegisterEvent(
        //    this._eventId, this._onEventRoot);
        wr.jdn(this._eventId, this._onEventRoot);
}
