// AnomalyServices.cs
// ─ hookName(문자열) → 등록된 핸들러 실행
using System;
using System.Collections.Generic;
using UnityEngine;

public static class AnomalyServices
{
    // 기본: 컨텍스트만 받는 훅
    private static readonly Dictionary<string, Action<AnomalyContext>> _handlers
        = new(StringComparer.Ordinal);

    // (선택) 페이로드까지 받는 훅
    private static readonly Dictionary<string, Action<AnomalyContext, object>> _handlersWithPayload
        = new(StringComparer.Ordinal);

    // 등록/해제
    public static void Register(string hookName, Action<AnomalyContext> handler)
    {
        if (string.IsNullOrWhiteSpace(hookName) || handler == null) return;
        _handlers[hookName] = handler;
    }
    public static void Register(string hookName, Action<AnomalyContext, object> handler)
    {
        if (string.IsNullOrWhiteSpace(hookName) || handler == null) return;
        _handlersWithPayload[hookName] = handler;
    }
    public static void Unregister(string hookName)
    {
        _handlers.Remove(hookName);
        _handlersWithPayload.Remove(hookName);
    }

    // 호출(컨텍스트만)
    public static bool Invoke(string hookName, AnomalyContext ctx, bool warnIfMissing = true)
    {
        if (_handlers.TryGetValue(hookName, out var h))
        {
            try { h(ctx); return true; }
            catch (Exception e) { Debug.LogException(e); return false; }
        }
        if (warnIfMissing) Debug.LogWarning($"[AnomalyServices] No handler for '{hookName}'");
        return false;
    }

    // 호출(페이로드 포함)
    public static bool Invoke(string hookName, AnomalyContext ctx, object payload, bool warnIfMissing = true)
    {
        if (_handlersWithPayload.TryGetValue(hookName, out var h))
        {
            try { h(ctx, payload); return true; }
            catch (Exception e) { Debug.LogException(e); return false; }
        }
        // 컨텍스트만 받는 핸들러가 있으면 그것도 시도(유연성)
        if (_handlers.TryGetValue(hookName, out var hCtxOnly))
        {
            try { hCtxOnly(ctx); return true; }
            catch (Exception e) { Debug.LogException(e); return false; }
        }
        if (warnIfMissing) Debug.LogWarning($"[AnomalyServices] No handler(p) for '{hookName}'");
        return false;
    }

    // (선택) 진단용: 현재 등록된 훅 나열
    public static IEnumerable<string> ListHooks()
    {
        foreach (var k in _handlers.Keys) yield return k;
        foreach (var k in _handlersWithPayload.Keys) yield return k + " (payload)";
    }
}
