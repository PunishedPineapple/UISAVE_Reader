# UISAVE_Reader
Processes the FFXIV UISAVE.DAT file format.

## Game Data Format
* 16-byte header.  First eight bytes possibly contain a file format version.  Bytes 8-11 are a 32-bit integer containing the number of valid bytes in the file *after* the header.  Bytes 12-15 are of unknown use.
* After the header, valid regions of the file are XOR-encoded by 0x31.
* Multi-byte data is little-endian.
* First eight bytes unknown.
* Next eight bytes are the character's 64-bit integer Content ID.
* The remainder of the valid data in the file is arranged in sections.  These are formatted as follows:
** 16-byte header.  First two bytes are section index, starting at 0.  The next six bytes are of unknown use.  Byte 8 of the section header is the length (in bytes) of the section's data (excluding the header).  The remaining four header bytes are of unknown purpose.
** Data.
** Four trailing bytes.  These seem to always be zero. Possibly reserved for a checksum, but unsure.
** Some sections start with a 0xFF byte, and these need to be further XOR'd by 0x73 to read.
* With this format, each section must be processed to find the next section due to arbitrary lengths.

## Sections
Sections map out in memory to the following (in-order):
* LETTER.DAT
* RETTASK.DAT
* FLAGS.DAT
* RCFAV.DAT
* UIDATA.DAT
* TLPH.DAT
* ITCC.DAT
* PVPSET.DAT
* EMTH.DAT
* MNONLST.DAT
* MUNTLST.DAT
* EMJ.DAT
* AOZNOTE.DAT
* CWLS.DAT
* ACHVLST.DAT
* GRPPOS.DAT
* CRAFT.DAT
* FMARKER.DAT
(Thank you Wintermute)

Rough information about each section format below.

### Mail History (Section 0x0, LETTER.DAT)
Sent letter history.  First 4 bytes unknown, possibly duplicate section ID.  Next 4 bytes likely number of valid entries. Next 8 bytes unkown, possibly padding.  Entries: 128 Bytes total per entry: 36? Bytes for recipient name, four bytes for gil amount, four byte unix timestamp, four bytes item number 1 id (HQ encoded by adding 1000000), four bytes quantity item number 1, repeat items 2-5, last 44 bytes unknown.
### Social (Section 0x4, UIDATA.DAT)
Starts with friends list group titles.  Also contains recent contacts list, as well as lists at least related to, if not copies of, friends list and blacklist.  Entries of friends are name, then eight byte content ID, then server ID.
### Teleport History (Section 0x5, TLPH.DAT)
Aetheryte Teleport History.  0x73-encoded, First byte aetheryte number, followed by five empty bytes, repeated for 20 total entries.
### CWLS (Section 0xD, CWLS.DAT)
Contains CWLS slot assignments, unsure if anything else.
### Field Markers (Section 0x11, FMARKER.DAT)
Saved waymark presets.  See the [WaymarkLibrarian](https://github.com/PunishedPineapple/WaymarkLibrarian#game-data-format) information for data format.  Starts 16 bytes into the section.
