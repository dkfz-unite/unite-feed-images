# Image Model
Includes basic image information.

>[!NOTE]
> All exact dates are hiddent and protected. Relative dates are shown instead, if calculation was possible.

**`id`*** - Image identifier.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"MRI1"`

**`donor_id`*** - Image donor identifier.
- Type: _String_
- Limitations: Maximum length 255
- Example: `"Donor1"`

**`creation_date`** - Date, when image was created.
- Type: _String_
- Format: "YYYY-MM-DD"
- Limitations: Only either 'creation_date' or 'creation_day' can be set at once, not both
- Example: `"2020-01-07"`

**`creation_day`** - Relative number of days since diagnosis statement, when image was created.
- Type: _Number_
- Limitations: Integer, greater or equal to 1, only either 'creation_date' or 'creation_day' can be set at once, not both
- Example: `7`


##
**`*`** - Required fields
