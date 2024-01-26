# Image Data Models
Images upload data model.

**`id`*** - Image identifier.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"IM1"`

**`donor_id`*** - Image donor identifier.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"Donor1"`

**`scanning_date`** - Date, when image was created.
- Note: It's hidden and protected. Relative date is shown instead, if calculation was possible.
- Type: _String_
- Format: "YYYY-MM-DD"
- Limitations: Only either 'ScanningDate' or 'ScanningDay' can be set at once, not both
- Example: `"2020-01-07"`

**`scanning_day`** - Relative number of days since diagnosis statement, when image was created.
- Type: _Number_
- Limitations: Integer, greater or equal to 0, only either 'ScanningDate' or 'ScanningDay' can be set at once, not both
- Example: `7`

**`mri_image`*** - MRI image data.
- Type: _Object([mri_image](./api-models-base-mri.md))_
- Limitations - If set, at least one field has to be set
- Example: `{...}`

**`analysis`** - Image analysis and features data.
- Type: _Object([analysis](./api-models-base-analysis.md))_
- Limitations - If set, at least one field has to be set
- Example: `{...}`

##
**`*`** - Required fields
