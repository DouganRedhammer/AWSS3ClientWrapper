using Amazon.S3;
using Amazon.S3.Model;

namespace AWSS3ClientWrapper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            S3ClientWrapper s3Client = new S3ClientWrapper("REPLACE_WITH_YOUR_AWS_REGION", "<REPLACE_WITH_YOUR_BUCKET_NAME>");

            Console.WriteLine("List all of the S3 files.");
            var listing = s3Client.ListAllFiles().GetAwaiter().GetResult();
            foreach (var l in listing)
            {
                Console.WriteLine(l);
            }
        }
    }
}
