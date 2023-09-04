using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Ecommerce.Persistence.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ecommerce.Persistence.Services
{
	public class ProductImageStorageService : IStorageService
	{
		private readonly ILogger<ProductImageStorageService> _logger;
		private readonly IConfiguration _configuration;

		public ProductImageStorageService(ILogger<ProductImageStorageService> logger, IConfiguration configuration)
		{
			this._logger = logger;
			this._configuration = configuration;
		}
		
		public async Task<bool> UploadFileAsync(string remoteStoragePath, string fileName, Stream file)
		{
			//Add credentials
			BasicAWSCredentials credentials = new BasicAWSCredentials(this._configuration["AWSCredentials:AWSAccessKey"], this._configuration["AWSCredentials:AWSSecretKey"]);

			//Setup configuration
			AmazonS3Config config = new AmazonS3Config()
			{
				RegionEndpoint = Amazon.RegionEndpoint.USEast1
			};

			try
			{
				//Create the upload request
				TransferUtilityUploadRequest uploadRequest = new TransferUtilityUploadRequest()
				{
					InputStream = file,
					Key = fileName,
					BucketName = remoteStoragePath
				};

				//Create the client and upload the file
				using (AmazonS3Client client = new AmazonS3Client(credentials, config))
				{
					TransferUtility transferUtility = new TransferUtility(client);
					await transferUtility.UploadAsync(uploadRequest);
				}
			}
			catch (AmazonS3Exception s3Exception)
			{
				this._logger.LogError(s3Exception, "S3 error uploading file to s3");
				return false;
			}
			catch (Exception e)
			{
				this._logger.LogError(e, "Error uploading file to s3");
				return false;
			}

			return true;
		}

		public async Task<bool> DeleteFileAsync(string remoteStoragePath, string fileName)
		{
			//Add credentials
			BasicAWSCredentials credentials = new BasicAWSCredentials(this._configuration["AWSCredentials:AWSAccessKey"], this._configuration["AWSCredentials:AWSSecretKey"]);

			//Setup configuration
			AmazonS3Config config = new AmazonS3Config()
			{
				RegionEndpoint = Amazon.RegionEndpoint.USEast1
			};

			try
			{
				//Create client and delete file
				using (AmazonS3Client client = new AmazonS3Client(credentials, config))
				{
					TransferUtility transferUtility = new TransferUtility(client);

					await transferUtility.S3Client.DeleteObjectAsync(new DeleteObjectRequest
					{
						BucketName = remoteStoragePath,
						Key = fileName
					});
				}
			}
			catch (AmazonS3Exception s3Exception)
			{
				this._logger.LogError(s3Exception, "S3 error deleting file from s3");
				return false;
			}
			catch (Exception e)
			{
				this._logger.LogError(e, "Error deleting file from s3");
				return false;
			}

			return true;
		}
	}
}