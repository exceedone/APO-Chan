/*
 * Reference from: 
 * https://github.com/XLabs/Xamarin-Forms-Labs/wiki/Geolocator
 */

using Plugin.Geolocator.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Apo_Chan.Geolocation
{
	/// <summary>
	///     Interface IGeolocator
	/// </summary>
	public interface IFusedGeolocator
	{
		/// <summary>
		///     Gets or sets the desired accuracy.
		/// </summary>
		/// <value>The desired accuracy.</value>
		double DesiredAccuracy { get; set; }

		/// <summary>
		///     Gets a value indicating whether [supports heading].
		/// </summary>
		/// <value><c>true</c> if [supports heading]; otherwise, <c>false</c>.</value>
		bool SupportsHeading { get; }

		/// <summary>
		///     Gets a value indicating whether this instance is geolocation available.
		/// </summary>
		/// <value><c>true</c> if this instance is geolocation available; otherwise, <c>false</c>.</value>
		bool IsGeolocationAvailable { get; }

		/// <summary>
		///     Gets a value indicating whether this instance is geolocation enabled.
		/// </summary>
		/// <value><c>true</c> if this instance is geolocation enabled; otherwise, <c>false</c>.</value>
		bool IsGeolocationEnabled { get; }

		/// <summary>
		///     Occurs when [position error].
		/// </summary>
		event EventHandler<PositionErrorEventArgs> PositionError;

		/// <summary>
		///     Occurs when [position changed].
		/// </summary>
		event EventHandler<PositionEventArgs> PositionChanged;

		/// <summary>
		///     Gets the position asynchronous.
		/// </summary>
		/// <param name="timeout">The timeout.</param>
		/// <returns>Task&lt;Position&gt;.</returns>
		Task<Position> GetPositionAsync(int timeout);

		/// <summary>
		///     Gets the position asynchronous.
		/// </summary>
		/// <param name="timeout">The timeout.</param>
		/// <param name="includeHeading">if set to <c>true</c> [include heading].</param>
		/// <returns>Task&lt;Position&gt;.</returns>
		Task<Position> GetPositionAsync(int timeout, bool includeHeading);

		/// <summary>
		///     Gets the position asynchronous.
		/// </summary>
		/// <param name="cancelToken">The cancel token.</param>
		/// <returns>Task&lt;Position&gt;.</returns>
		Task<Position> GetPositionAsync(CancellationToken cancelToken);

		/// <summary>
		///     Gets the position asynchronous.
		/// </summary>
		/// <param name="cancelToken">The cancel token.</param>
		/// <param name="includeHeading">if set to <c>true</c> [include heading].</param>
		/// <returns>Task&lt;Position&gt;.</returns>
		Task<Position> GetPositionAsync(CancellationToken cancelToken, bool includeHeading);

		/// <summary>
		///     Gets the position asynchronous.
		/// </summary>
		/// <param name="timeout">The timeout.</param>
		/// <param name="cancelToken">The cancel token.</param>
		/// <returns>Task&lt;Position&gt;.</returns>
		Task<Position> GetPositionAsync(int timeout, CancellationToken cancelToken);

		/// <summary>
		///     Gets the position asynchronous.
		/// </summary>
		/// <param name="timeout">The timeout.</param>
		/// <param name="cancelToken">The cancel token.</param>
		/// <param name="includeHeading">if set to <c>true</c> [include heading].</param>
		/// <returns>Task&lt;Position&gt;.</returns>
		Task<Position> GetPositionAsync(int timeout, CancellationToken cancelToken, bool includeHeading);
	}
}