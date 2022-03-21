# Image Data Models

## Image
Includes general data about the image

**`Id`*** - Image identifier.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"IM1"`

**`DonorId`*** - Image donor identifier.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"DO1"`

**`ScanningDate`** - Date, when image was created.
- Note: It's hidden and protected. Relative date is shown instead, if calculation was possible.
- Type: _String_
- Format: "YYYY-MM-DDTHH:MM:SS"
- Limitations: Only either 'ScanningDate' or 'ScanningDay' can be set at once, not both
- Example: `"2020-01-07T00:00:00"`

**`ScanningDay`** - Relative number of days since diagnosis statement, when image was created.
- Type: _Number_
- Limitations: Integer, greater or equal to 0, only either 'ScanningDate' or 'ScanningDay' can be set at once, not both
- Example: `7`

**`MriImage`*** - MRI image data.
- Type: _Object([MriImage](https://github.com/dkfz-unite/unite-images-feed/blob/main/Docs/api-images-models-mri.md))_
- Limitations - If set, at least one field has to be set
- Example: `{...}`

**`Analysis`** - Image analysis and features data.
- Type: _Object([Analysis](https://github.com/dkfz-unite/unite-images-feed/blob/main/Docs/api-images-models-analysis.md))_
- Limitations - If set, at least one field has to be set
- Example: `{...}`

##
**`*`** - Required fields
