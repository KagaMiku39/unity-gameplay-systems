using System;

public sealed class ShopTransactionSucceededEvent
{
    public ShopTransactionResult Result { get; }

    public ShopTransactionSucceededEvent(
        ShopTransactionResult result
    )
    {
        Result = result
            ?? throw new ArgumentNullException(nameof(result));

        if (Result.IsSuccess == false)
        {
            throw new ArgumentException(
                "只有成功或部分成功的交易才能发布成功事件。",
                nameof(result)
            );
        }
    }
}
