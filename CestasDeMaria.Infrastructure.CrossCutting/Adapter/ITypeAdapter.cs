﻿namespace CestasDeMaria.Infrastructure.CrossCutting.Adapter
{
    public interface ITypeAdapter
    {
        /// <summary>
        ///  Adapt a source object to an instance of type TTarget
        /// </summary>
        /// <typeparam name="TSource"> Type of source item </typeparam>
        /// <typeparam name="TTarget"> Type of target item </typeparam>
        /// <param name="source"> Instance to adapt </param>
        /// <returns> <paramref name="source" /> mapped to <typeparamref name="TTarget" /> </returns>
        TTarget Adapt<TSource, TTarget>(TSource source) where TTarget : class, new() where TSource : class;

        /// <summary>
        /// Adapt a source object to an instnace of type TTarget
        /// </summary>
        /// <typeparam name="TTarget">Type of target item</typeparam>
        /// <param name="source">Instance to adapt</param>
        /// <returns> <paramref name="source" /> mapped to <typeparamref name="TTarget" /> </returns>
        TTarget Adapt<TTarget>(object source) where TTarget : class;
    }
}
