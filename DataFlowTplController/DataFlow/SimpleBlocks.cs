using System.Threading.Tasks.Dataflow;

namespace DataFlowTplController.DataFlow;

public class SimpleBlocks
{
    public async Task Process()
    {
        var transformBlock = new TransformBlock<int, string>(async item =>
        {
            await Task.Delay(10);
            // Process and transform the item
            Console.WriteLine("Enter in block");
            return item.ToString();
        }, new ExecutionDataflowBlockOptions
        {
            EnsureOrdered = true,
            //BoundedCapacity = 5
            //MaxDegreeOfParallelism = 4
        });

        var actionBlock = new ActionBlock<string>(async item =>
        {
            try
            {
                //await Task.Delay(100);
                // Perform action on the item
                Console.WriteLine($"Processing {item}");
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                Console.WriteLine($"Error processing {item}: {ex.Message}");
            }
        }, new ExecutionDataflowBlockOptions
        {
            EnsureOrdered = true,
            MaxDegreeOfParallelism = 4,
            // BoundedCapacity = 4,
        });

        transformBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });

// Post items to the transformBlock
        for (int i = 0; i < 100; i++)
        {
            transformBlock.Post(i);
        }

// Complete the transformBlock
        //transformBlock.Complete();

// Await the completion of the actionBlock
        //await actionBlock.Completion;

        await Task.Delay(TimeSpan.FromMinutes(1));
    }
}