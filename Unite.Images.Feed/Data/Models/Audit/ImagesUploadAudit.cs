using System.Collections.Generic;
using System.Text;

namespace Unite.Images.Feed.Data.Models.Audit
{
    public class ImagesUploadAudit
    {
        public int DonorsCreated;
        public int ImagesCreated;
        public int ImagesUpdated;
        public int ImagesAnalysed;

        public HashSet<int> Images;


        public ImagesUploadAudit()
        {
            Images = new HashSet<int>();
        }


        public override string ToString()
        {
            var message = new StringBuilder();

            message.AppendLine($"{DonorsCreated} donors created");
            message.AppendLine($"{ImagesCreated} images created");
            message.AppendLine($"{ImagesUpdated} images updated");
            message.AppendLine($"{ImagesAnalysed} image analysed");

            return message.ToString();
        }
    }
}
