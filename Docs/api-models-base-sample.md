# Image Sample Model
Includes data about image and it's analysis.

>[!NOTE]
> All exact dates are hiddent and protected. Relative dates are shown instead, if calculation was possible.

**`donor_id`*** - Donor identifier whom the image belongs to.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"Donor1"`

**`image_id`*** - Image identifier.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"MRI1"`

**`image_type`*** - Image type.
- Type: _String_
- Possible values: `"MRI"`
- Example: `"MRI"`

**`analysis_type`*** - Image analysis type.
- Type: _String_
- Possible values: `"RFE"`
- Example: `"RFE"`

**`analysis_date`** - Image analysis date.
- Type: _String_
- Format: "YYYY-MM-DD"
- Limitations: Only either 'analysis_date' or 'analysis_day' can be set at once, not both
- Example: `"2019-01-01"`

**`analysis_day`** - Relative number of days since diagnosis statement, when analysis was performed.
- Type: _Number_
- Limitations:  Integer, greater or equal to 1, only either 'analysis_date' or 'analysis_day' can be set at once, not both
- Example: `1`


#### Image Type
Supported image types are:
- `"MRI"` - Magnetic Resonance Imaging

#### Analysis Type
Supported analysis types are:
- `"RFE"` - Radiomics Features Extraction


##
**`*`** - Required fields
