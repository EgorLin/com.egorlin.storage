using System;

namespace EgorLin.Storage.Time
{
	public interface IProviderTime
	{
		float DeltaTick { get; }
		DateTime TimeNow { get; }
		DateTime TimeNowUtc { get; }
	}
}
