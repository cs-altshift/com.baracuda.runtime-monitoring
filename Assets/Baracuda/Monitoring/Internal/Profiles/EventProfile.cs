﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Baracuda.Attributes;
using Baracuda.Attributes.Monitoring;
using Baracuda.Monitoring.Internal.Reflection;
using Baracuda.Monitoring.Internal.Units;
using Baracuda.Monitoring.Internal.Utils;
using Baracuda.Pooling.Concretions;

namespace Baracuda.Monitoring.Internal.Profiles
{
    public class EventProfile<TTarget, TDelegate> : MonitorProfile where TDelegate : MulticastDelegate where TTarget : class
    {
        #region --- [FIELDS] ---

        public delegate string StateFormatDelegate(TTarget target, int invokeCount);

        internal readonly MonitorEventAttribute EventAttribute;
        private readonly EventInfo _eventInfo;
        private readonly StateFormatDelegate _formatState;
        private readonly Action<TTarget, Delegate> _subscribe;
        private readonly Action<TTarget, Delegate> _remove;
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [FACTORY] ---

        public override MonitorUnit CreateUnit(object target)
        {
            return new EventUnit<TTarget, TDelegate>((TTarget) target, _formatState, this);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [CTOR] ---
        
        public EventProfile(EventInfo eventInfo, MonitorEventAttribute attribute, MonitorProfileCtorArgs args) 
            : base(eventInfo, attribute, typeof(TTarget), typeof(TDelegate), UnitType.Event, args)
        {
            _eventInfo = eventInfo;
            EventAttribute = attribute;
            
            var addMethod = eventInfo.GetAddMethod(true);
            var removeMethod = eventInfo.GetRemoveMethod(true);

            _subscribe = CreateExpression(addMethod).Compile();
            _remove = CreateExpression(removeMethod).Compile();
            
            var getterDelegate = eventInfo.AsFieldInfo().CreateGetter<TTarget, Delegate>();
            var counterDelegate = CreateCounterExpression(getterDelegate, attribute.ShowTrueCount).Compile();
            _formatState = CreateStateFormatter(counterDelegate);
        }
        
        private static Expression<Action<TTarget, Delegate>> CreateExpression(MethodInfo methodInfo)
        {
            return (target, @delegate) => methodInfo.Invoke(target, new object[]{@delegate});
        }
        
        private static Expression<Func<TTarget, int>> CreateCounterExpression(Func<TTarget, Delegate> func, bool trueCount)
        {
            if(trueCount) return  target => func(target).GetInvocationList().Length;
            
            return target => func(target).GetInvocationList().Length - 1;
        }
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------   

        #region --- [STATE FORMATTER] ---

        private StateFormatDelegate CreateStateFormatter(Func<TTarget, int> counterDelegate)
        {
            if (EventAttribute.ShowSignature)
            {
                var parameterString = _eventInfo.GetEventSignatureString();
                return (target, count) =>
                {
                    var sb = StringBuilderPool.Get();
                    sb.Append(parameterString);
                    sb.Append(" Subscriber:");
                    sb.Append(counterDelegate(target));
                    sb.Append(" Invokes: ");
                    sb.Append(count);
                    return StringBuilderPool.Release(sb);
                };
            }
            
            
            return (target, count) =>
            {
                var sb = StringBuilderPool.Get();
                sb.Append(Label);
                sb.Append(" Subscriber:");
                sb.Append(counterDelegate(target));
                sb.Append(" Invokes: ");
                sb.Append(count);
                return StringBuilderPool.Release(sb);
            };
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------   
        
        #region --- [EVENT HANDLER] ---

        internal void SubscribeEventHandler(TTarget target, Delegate eventHandler)
        {
            _subscribe(target, eventHandler);
        }

        internal void RemoveEventHandler(TTarget target, Delegate eventHandler)
        {
            _remove(target, eventHandler);
        }
        
        internal Delegate CreateEventHandler(Action action)
        {
            var handlerType = _eventInfo.EventHandlerType;
            var eventParams = handlerType.GetMethod("Invoke").GetParameters();

            //lambda: (object x0, EventArgs x1) => action()
            var parameters = eventParams.Select(p=>Expression.Parameter(p.ParameterType,"x"));
            var body = Expression.Call(Expression.Constant(action),action.GetType().GetMethod("Invoke"));
            var lambda = Expression.Lambda(body,parameters.ToArray());
            return Delegate.CreateDelegate(handlerType, lambda.Compile(), "Invoke", false);
        }
        
        #endregion
    }
}