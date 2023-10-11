# Images Data Feed API

## GET: [api](http://localhost:5102/api) - [api/images-feed](https://localhost/api/images-feed)
Health check.

### Responses
`"2022-03-17T09:45:10.9359202Z"` - Current UTC date and time in JSON format, if service is up and running


## POST: [api/images](http://localhost:5102/api/images) - [api/images-feed](https://localhost/api/images-feed)
Submit images data (including image analysis data).

Request implements **UPSERT** logic:
- Missing data will be populated
- Existing data will be updated

### Headers
- `Authorization: Bearer [token]` - JWT token with `Data.Write` permission.

### Boby - application/json
```json
[
    {
        "id": "IM1",
        "donor_id": "DO1",
        "scanning_date": null,
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
            "id": "IA1IM1",
            "type": "RFE",
            "parameters": {
                "diagnostics_Versions_PyRadiomics": "v3.0.1",
                "diagnostics_Versions_Numpy": "1.19.5",
                "diagnostics_Versions_SimpleITK": "2.0.2",
                "diagnostics_Versions_PyWavelet": "1.1.1",
                "diagnostics_Versions_Python": "3.6.12",
                "diagnostics_Configuration_Settings": "{'minimumROIDimensions': 2, 'minimumROISize': None, 'normalize': False, 'normalizeScale': 1, 'removeOutliers': None, 'resampledPixelSpacing': None, 'interpolator': 'sitkBSpline', 'preCrop': False, 'padDistance': 5, 'distances': [1], 'force2D': False, 'force2Ddimension': 0, 'resegmentRange': None, 'label': 1, 'additionalInfo': True, 'binWidth': 25, 'weightingNorm': None}",
                "diagnostics_Configuration_EnabledImageTypes": "{'Original': {}}"
            },
            "features": {
                "diagnostics_Image-original_Hash": "621f7c6ccb82e8694bde90e0918ada64f473799d",
                "diagnostics_Image-original_Dimensionality": "3D",
                "diagnostics_Image-original_Spacing": "(1.0, 1.0, 1.0)",
                "diagnostics_Image-original_Size": "(160, 226, 250)",
                "diagnostics_Image-original_Mean": "48.985951880134124",
                "diagnostics_Image-original_Minimum": "0.0",
                "diagnostics_Image-original_Maximum": "621.5372314453125",
                "diagnostics_Mask-original_Hash": "a891129e57bc05be8b94a20aa0b42977fc0143b2",
                "diagnostics_Mask-original_Spacing": "(1.0, 1.0, 1.0)",
                "diagnostics_Mask-original_Size": "(160, 226, 250)",
                "diagnostics_Mask-original_BoundingBox": "(79, 47, 161, 54, 116, 53)",
                "diagnostics_Mask-original_VoxelNum": "111393",
                "diagnostics_Mask-original_VolumeNum": "1",
                "diagnostics_Mask-original_CenterOfMassIndex": "(101.19481475496664, 103.53967484491844, 184.48638603862003)",
                "diagnostics_Mask-original_CenterOfMass": "(-25.56927866847508, -17.731195431161368, 37.160734646633585)",
                "original_shape_MeshVolume": "111280.45833333333",
                "original_ngtdm_Strength": "0.05258557801305185"
            }
        }
    }
]
```
Fields description can be found [here](api-images-models.md).

### Responses
- `200` - request was processed successfully
- `400` - request data didn't pass validation
- `401` - missing JWT token
- `403` - missing required permissions
