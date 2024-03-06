using Amazon.S3;
using Amazon.S3.Model;

namespace AWSS3ClientWrapper
{
    public class S3ClientWrapper
    {
        private AmazonS3Client _s3Client;
        private string _bucket;

        public S3ClientWrapper(string region, string bucket)
        {
            _s3Client = new AmazonS3Client(Amazon.RegionEndpoint.GetBySystemName(region));
            _bucket = bucket;
        }

        public async Task<List<string>> ListAllFiles()
        {
            var listing = new List<String>();

            var request = new ListObjectsV2Request
            {
                BucketName = _bucket
            };

            ListObjectsV2Response response;
            do
            {
                response = await _s3Client.ListObjectsV2Async(request);

                foreach (S3Object entry in response.S3Objects)
                {
                    listing.Add(entry.Key);
                }

                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);

            return listing;
        }

        public async Task<string> GetFile(string key)
        {
            var result = string.Empty;

            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = _bucket,
                    Key = key
                };

                using var response = await _s3Client.GetObjectAsync(request);
                using var reader = new StreamReader(response.ResponseStream);
                result = await reader.ReadToEndAsync();
            }
            catch (AmazonS3Exception e)
            {
                //Something that should be logged to a file
                Console.WriteLine("Error encountered on server. Message:'{0}' when getting an object", e.Message);
            }
            catch (Exception e)
            {
                //Something that should be logged to a file
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when getting an object", e.Message);
            }
            return !String.IsNullOrEmpty(result) ? result : "File not found." ;
        }

        public async Task DeleteFile(string key)
        {
            try
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = _bucket,
                    Key = key
                };

                await _s3Client.DeleteObjectAsync(request);
            }
            catch (AmazonS3Exception e)
            {
                //Something that should be logged to a file
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                //Something that should be logged to a file
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }
    

        public async Task<bool> CopyFile(string sourceKey, string targetKey)
        {
            try
            {
                var request = new CopyObjectRequest
                {
                    SourceBucket = _bucket,
                    SourceKey = sourceKey,
                    DestinationBucket = _bucket,
                    DestinationKey = targetKey
                };

                await _s3Client.CopyObjectAsync(request);
            }
            catch (AmazonS3Exception e)
            {
                //Something that should be logged to a file
                Console.WriteLine("Error encountered on server. Message:'{0}' when copying the object", e.Message);
                return false;
            }
            catch (Exception e)
            {
                //Something that should be logged to a file
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when copying the object", e.Message);
                return false;
            }

            return true;
        }

        public async Task<DateTime> GetLastModifiedDate(string key)
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _bucket,
                Key = key
            };

            var response = await _s3Client.GetObjectMetadataAsync(request);
            return response.LastModified;
        }

        public async Task<bool> UploadFile(string filePath, string key)
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucket,
                Key = key,
                FilePath = filePath
            };

            var response = await _s3Client.PutObjectAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Successfully uploaded {key} to {_bucket}.");
                return true;
            }
            else
            {
                Console.WriteLine($"Could not upload {key} to {_bucket}.");
                return false;
            }
        }
    }
}