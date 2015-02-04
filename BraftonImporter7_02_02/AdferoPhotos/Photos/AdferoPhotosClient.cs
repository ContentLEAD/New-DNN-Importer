using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdferoVideoDotNet.AdferoPhotos.Photos
{
    public class AdferoPhotosClient
    {
        private string baseUri;

        public AdferoPhotosClient(string baseUri)
        {
            this.baseUri = baseUri;
        }

        public AdferoPhoto GetLocationUrl(int id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException("id must be a positive integer.");

            string locationUri = this.GetPhotoBaseUri(id);
            AdferoPhoto photo = new AdferoPhoto();
            photo.Id = id;
            photo.LocationUri = locationUri;

            return photo;
        }

        public AdferoPhoto GetCropLocationUrl(int id, int? cropWidth, int? cropHeight, int? focalPointX, int? focalPointY)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException("id", "id must be a positive integer.");

            if (cropWidth.HasValue && cropWidth.Value <= 0)
                throw new ArgumentOutOfRangeException("cropWidth", "cropWidth must be greater than 0.");

            if (cropHeight.HasValue && cropHeight.Value <= 0)
                throw new ArgumentOutOfRangeException("cropHeight", "cropHeight must be greater than 0.");

            // editor's note: zeroes are neither positive nor negative,
            // but in the interest of smooth porting we're keeping error messages the same.
            if (focalPointX.HasValue && focalPointX.Value < 0)
                throw new ArgumentOutOfRangeException("focalPointX", "focalPointX must be a positive integer.");

            if (focalPointY.HasValue && focalPointY.Value < 0)
                throw new ArgumentOutOfRangeException("focalPointY", "focalPointY must be a positive integer.");

            string uri = this.GetPhotoBaseUri(id);
            Dictionary<string, string> data = new Dictionary<string, string>();

            data["action"] = "crop";

            string cw = cropWidth.HasValue ? cropWidth.Value.ToString() : "*";
            string ch = cropHeight.HasValue ? cropHeight.Value.ToString() : "*";
            data["crop"] = string.Format("{0}x{1}", cw, ch);

            if (focalPointX.HasValue || focalPointY.HasValue)
            {
                string fpx = focalPointX.HasValue ? focalPointX.Value.ToString() : "*";
                string fpy = focalPointY.HasValue ? focalPointY.Value.ToString() : "*";
                data["focalpoint"] = string.Format("{0}x{1}", fpx, fpy);
            }

            List<string> parts = new List<string>();
            foreach (KeyValuePair<string, string> kv in data)
                parts.Add(string.Format("{0}={1}", kv.Key, kv.Value));
            string queryString = HttpUtility.UrlDecode(string.Join("&", parts.ToArray()));
            string locationUri = string.Format("{0}?{1}", uri, queryString);

            AdferoPhoto photo = new AdferoPhoto();
            photo.Id = id;
            photo.LocationUri = locationUri;

            return photo;
        }

        public AdferoPhoto GetScaleLocationUrl(int id, string scaleAxis, int scale)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException("id", "id must be a positive integer.");

            // editor's note: yes, that says "must be a greater".
            // can't fix it unless the original library does too.
            if (scale <= 0)
                throw new ArgumentOutOfRangeException("scale", "scale must be a greater than 0.");

            string uri = this.GetPhotoBaseUri(id);
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["action"] = "scale";

            if (scaleAxis == AdferoScaleAxis.X)
                data["scale"] = string.Format("{0}x*", scale);
            else if (scaleAxis == AdferoScaleAxis.Y)
                data["scale"] = string.Format("*x{0}", scale);

            List<string> parts = new List<string>();
            foreach (KeyValuePair<string, string> kv in data)
                parts.Add(string.Format("{0}={1}", kv.Key, kv.Value));
            string queryString = HttpUtility.UrlDecode(string.Join("&", parts.ToArray()));
            string locationUri = string.Format("{0}?{1}", uri, queryString);

            AdferoPhoto photo = new AdferoPhoto();
            photo.Id = id;
            photo.LocationUri = locationUri;

            return photo;
        }

        public AdferoPhoto GetCropScaleLocationUrl(int id, int? cropWidth, int? cropHeight, int? focalPointX, int? focalPointY, string scaleAxis, int scale)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException("id", "id must be a positive integer.");

            if (cropWidth.HasValue && cropWidth.Value <= 0)
                throw new ArgumentOutOfRangeException("cropWidth", "cropWidth must be greater than 0.");

            if (cropHeight.HasValue && cropHeight.Value <= 0)
                throw new ArgumentOutOfRangeException("cropHeight", "cropHeight must be greater than 0.");

            // editor's note: zeroes are neither positive nor negative,
            // but in the interest of smooth porting we're keeping error messages the same.
            if (focalPointX.HasValue && focalPointX.Value < 0)
                throw new ArgumentOutOfRangeException("focalPointX", "focalPointX must be a positive integer.");

            if (focalPointY.HasValue && focalPointY.Value < 0)
                throw new ArgumentOutOfRangeException("focalPointY", "focalPointY must be a positive integer.");

            // editor's note: yes, that says "must be a greater".
            // can't fix it unless the original library does too.
            if (scale <= 0)
                throw new ArgumentOutOfRangeException("scale", "scale must be a greater than 0.");

            string uri = this.GetPhotoBaseUri(id);
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["action"] = "cropscale";

            string cw = cropWidth.HasValue ? cropWidth.Value.ToString() : "*";
            string ch = cropHeight.HasValue ? cropHeight.Value.ToString() : "*";
            data["crop"] = string.Format("{0}x{1}", cw, ch);

            if (focalPointX.HasValue || focalPointY.HasValue)
            {
                string fpx = focalPointX.HasValue ? focalPointX.Value.ToString() : "*";
                string fpy = focalPointY.HasValue ? focalPointY.Value.ToString() : "*";
                data["focalpoint"] = string.Format("{0}x{1}", fpx, fpy);
            }

            if (scaleAxis == AdferoScaleAxis.X)
                data["scale"] = string.Format("{0}x*", scale);
            else if (scaleAxis == AdferoScaleAxis.Y)
                data["scale"] = string.Format("*x{0}", scale);

            List<string> parts = new List<string>();
            foreach (KeyValuePair<string, string> kv in data)
                parts.Add(string.Format("{0}={1}", kv.Key, kv.Value));
            string queryString = HttpUtility.UrlDecode(string.Join("&", parts.ToArray()));
            string locationUri = string.Format("{0}?{1}", uri, queryString);

            AdferoPhoto photo = new AdferoPhoto();
            photo.Id = id;
            photo.LocationUri = locationUri;

            return photo;
        }

        private string GetPhotoBaseUri(int id)
        {
            return string.Format("{0}photo/{1}.jpg", this.baseUri, id);
        }
    }
}