# Images Data Feed API
Allows to submit images data to the repository.

> [!Note]
> API is accessible for authorized users only and requires `JWT` token as `Authorization` header (read more about [Identity Service](https://github.com/dkfz-unite/unite-identity)).

API is **proxied** to main API and can be accessed at [[host]/api/images-feed](http://localhost/api/images-feed) (**without** `api` prefix).

All data submision request implement **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated
- Redundand data will be removed


## Overview
- get:[api](#get-api) - health check.
- post:[api/images](#post-apiimages) - submit all images data.
- post:[api/mris/{type?}](#post-apimristype) - submit MRI images data in given type.

> [!Note]
> **Json** is default data type for all requests and will be used if no data type is specified.


## GET: [api](http://localhost:5102/api)
Health check.

### Responses
`"2022-03-17T09:45:10.9359202Z"` - Current UTC date and time in JSON format, if service is up and running


## POST: [api/images](http://localhost:5102/api/images)
Submit images data (including image analysis data).

### Boby
Supported formats are:
- `json` (**empty**) - application/json

##### json - application/json
```json
[
  {
    "id": "MRI1",
    "donor_id": "Donor1",
    "scanning_date": "2020-01-07",
    "scanning_day": 14,
    "mri_image": {
        "whole_tumor": 111.393,
        "contrast_enhancing": 902.000,
        "non_contrast_enhancing": 102.683,
        "median_adc_tumor": 1314.861083984375,
        "median_adc_ce": 1598.30419921875,
        "median_adc_edema": 1299.1142578125,
        "median_cbf_tumor": 23.221034049987793,
        "median_cbf_ce": 23.221034049987793,
        "median_cbf_edema": 23.221034049987793,
        "median_cbv_tumor": 311.923828125,
        "median_cbv_ce": 359.912109375,
        "median_cbv_edema": 327.919921875,
        "median_mtt_tumor": 2599.365234375,
        "median_mtt_ce": 2791.318359375,
        "median_mtt_edema": 2631.357421875
    },
    "analysis": {
        "id": "MRI1-Analysis1",
        "type": "RFE",
        "parameters": {
            "param_a1": "value_a1",
            "param_b2": "value_b2"
        },
        "features": {
            "feature_a1": "feature_a1",
            "feature_b2": "feature_b2"
        }
    }
  }
]
```
Fields description can be found [here](api-models-images.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions


## POST: [api/mris/{type?}](http://localhost:5102/api/mris)
Submit MRI images data.

### Boby
Supported formats are:
- `tsv` - text/tab-separated-values

For `json` upload see [POST: api/images](#post-apiimages).

##### tsv - text/tab-separated-values
```tsv
id	donor_id	scanning_date	scanning_day	whole_tumor	contrast_enhancing	non_contrast_enhancing	median_adc_tumor	median_adc_ce	median_adc_edema	median_cbf_tumor	median_cbf_ce	median_cbf_edema	median_cbv_tumor	median_cbv_ce	median_cbv_edema	median_mtt_tumor	median_mtt_ce	median_mtt_edema
MRI1	Donor2	2020-01-07	14	111.393	902.000	102.683	1314.861083984375	1598.30419921875	1299.1142578125	23.221034049987793	23.221034049987793	23.221034049987793	311.923828125	359.912109375	327.919921875	2599.365234375	2791.318359375	2631.357421875
```
Fields description can be found [here](api-models-images.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions
