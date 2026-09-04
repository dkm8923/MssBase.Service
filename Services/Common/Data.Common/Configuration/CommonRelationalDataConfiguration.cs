using System.Text.Json;
using Data.Common.Models;
using Dto.Common.CommonRelationalData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Data;
using Shared.Logic.Common;
using static Shared.Logic.Common.Constants;

namespace Data.Common.Configuration;

public class CommonRelationalDataConfiguration : IEntityTypeConfiguration<CommonRelationalData>
{
    private readonly string _tableName = "CommonRelationalData";
    public void Configure(EntityTypeBuilder<CommonRelationalData> builder)
    {
        _setTableName(builder);

        builder.Property(t => t.CommonRelationalDataId).IsRequired();
        builder.ConfigureAuditFields();
        builder.Property(t => t.ReferenceType).HasMaxLength(256).IsRequired().IsUnicode(false);
        builder.Property(t => t.Json).HasMaxLength(4096).IsUnicode(true);
        
        _createPrimaryKey(builder);
        _createUniqueKey(builder);
        _createTableData(builder); 
    }

    private void _setTableName(EntityTypeBuilder<CommonRelationalData> builder)
    {
        builder.ToTable(_tableName);
    }

    private void _createPrimaryKey(EntityTypeBuilder<CommonRelationalData> builder)
    {
        builder.HasKey(e => e.CommonRelationalDataId);
    }
    private void _createUniqueKey(EntityTypeBuilder<CommonRelationalData> builder)
    {
        builder.HasIndex(e => e.ReferenceType).IsUnique().HasDatabaseName( DataUtilities.CreateUniqueKey(_tableName, "ReferenceType"));
    }

    private void _createTableData(EntityTypeBuilder<CommonRelationalData> builder) 
    {
        var records = new List<CommonRelationalData>();
        records.Add(_configurePersonTitle(builder));
        records.Add(_configurePersonSuffix(builder));
        records.Add(_configurePersonSex(builder));
        records.Add(_configurePersonEthnicity(builder));
        records.Add(_configurePersonGender(builder));
        records.Add(_configurePersonMaritalStatus(builder));
        records.Add(_configurePersonReligion(builder));
        records.Add(_configurePersonSexuality(builder));
        records.Add(_configurePersonLanguage(builder));
        
        records.Add(_configureAddressType(builder));
        records.Add(_configurePhoneNumberType(builder));
        records.Add(_configureUsaState(builder));
        records.Add(_configureCountry(builder));
        records.Add(_configureTimeZone(builder));

        var idx = 1;

        foreach (var record in records)
        {
            record.CommonRelationalDataId = idx++;
            DataUtilities.SetActiveFieldToTrueForEntity(record);
            DataUtilities.SetAuditFieldsForEntity(record);
        }

        builder.HasData(records);
    }

    #region Person

    private CommonRelationalData _configurePersonTitle(EntityTypeBuilder<CommonRelationalData> builder)
    {
        var relationalRecords = new List<CommonRelationalDataDto>();
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Mr.", Description = "Adult man (regardless of marital status)" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Mrs.", Description = "Adult woman (usually just for married women, widows, and divorcées)" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Ms.", Description = "Adult woman (regardless of marital status)" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Mx.", Description = "Nonbinary, gender-neutral (does not indicate gender)" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Miss", Description = "Formal title for unmarried women and for female children" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Madam", Description = "(also madame) – Adult woman" });
        
        DataUtilities.SetActiveFieldToTrue(relationalRecords);
        DataUtilities.SetAuditFields(relationalRecords);

        var ret = new CommonRelationalData { ReferenceType = CommonRelationalDataReferenceTypes.PersonTitle, Description = "Title for a person / contact / user", Json = JsonSerializer.Serialize(relationalRecords) };
        return ret;
    }

    private CommonRelationalData _configurePersonSuffix(EntityTypeBuilder<CommonRelationalData> builder)
    {
        var relationalRecords = new List<CommonRelationalDataDto>();
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Jr.", Description = "Junior" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Sr.", Description = "Senior" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "II", Description = "The second" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "III", Description = "The third" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "IV", Description = "The fourth" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "V", Description = "The fifth" });
        
        DataUtilities.SetActiveFieldToTrue(relationalRecords);
        DataUtilities.SetAuditFields(relationalRecords);

        var ret = new CommonRelationalData { ReferenceType = CommonRelationalDataReferenceTypes.PersonSuffix, Description = "Suffix for a person / contact / user", Json = JsonSerializer.Serialize(relationalRecords) };
        return ret;
    }

    private CommonRelationalData _configurePersonSex(EntityTypeBuilder<CommonRelationalData> builder)
    {
        var relationalRecords = new List<CommonRelationalDataDto>();
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Male" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Female" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Intersex" });
        
        DataUtilities.SetActiveFieldToTrue(relationalRecords);
        DataUtilities.SetAuditFields(relationalRecords);

        var ret = new CommonRelationalData { ReferenceType = CommonRelationalDataReferenceTypes.PersonSex, Description = "Sex for a person / contact / user", Json = JsonSerializer.Serialize(relationalRecords) };
        return ret;
    }

    private CommonRelationalData _configurePersonEthnicity(EntityTypeBuilder<CommonRelationalData> builder)
    {
        var relationalRecords = new List<CommonRelationalDataDto>();
        relationalRecords.Add(new CommonRelationalDataDto { Name = "American Indian or Alaska Native", Description = "A person having origins in any of the original peoples of North and South America (including Central America) and who maintains tribal affiliation or community attachment" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Asian", Description = "A person having origins in any of the original peoples of the Far East, Southeast Asia, or the Indian subcontinent including, for example, Cambodia, China, India, Japan, Korea, Malaysia, Pakistan, the Philippine Islands, Thailand, and Vietnam" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Black or African American", Description = "A person having origins in any of the Black racial groups of Africa" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Native Hawaiian or Other Pacific Islander", Description = "A person having origins in any of the original peoples of Hawaii, Guam, Samoa, or other Pacific Islands" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "White", Description = "A person having origins in any of the original peoples of Europe, the Middle East, or North Africa" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Hispanic or Latino", Description = "A person of Cuban, Mexican, Puerto Rican, South or Central American, or other Spanish culture or origin, regardless of race. The term, 'Spanish origin', can be used in addition to 'Hispanic or Latino'" });

        DataUtilities.SetActiveFieldToTrue(relationalRecords);
        DataUtilities.SetAuditFields(relationalRecords);

        var ret = new CommonRelationalData { ReferenceType = CommonRelationalDataReferenceTypes.PersonEthnicity, Description = "Ethnicity for a person / contact / user", Json = JsonSerializer.Serialize(relationalRecords) };
        return ret;
    }

    private CommonRelationalData _configurePersonGender(EntityTypeBuilder<CommonRelationalData> builder)
    {
        var relationalRecords = new List<CommonRelationalDataDto>();
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Male" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Female" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Agender", Description = "Not having a gender or identifying with a gender. They may describe themselves as being gender neutral or genderless" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Bigender", Description = "A person who fluctuates between traditionally “male” and “female” gender-based behaviours and identities" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Cisgender", Description = "A person whose gender identity and biological sex assigned at birth are the same. For example they were born biologically as a male, and express their gender as male" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Gender Fluid", Description = "A mix of boy and girl. A person who is gender fluid may always feel like a mix of the two traditional genders, but may feel more man some days, and more woman other days" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Genderqueer", Description = "A gender identity label often used by people who do not identify with being a man or a woman, or as an umbrella term for many gender non-conforming or non-binary identities" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Intersex", Description = "A person born with a reproductive or sexual anatomy that doesn’t seem to fit the typical definitions of female or male. For example, a person might be born appearing to be female on the outside, but having mostly male-typical anatomy on the inside" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Gender Variant", Description = "Someone who either by nature or by choice does not conform to gender-based expectations of society" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Third Gender", Description = "A term for a person who does not identify with either man or woman, but identifies with another gender. This gender category is used by societies that recognise three or more genders, both contemporary and historic, and is also a conceptual term meaning different things to different people who use it" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Transgender", Description = "A person who lives as a member of a gender other than that expected based on sex assigned at birth" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Two-Spirit", Description = "Is an umbrella term traditionally used by Native American people to recognise individuals who possess qualities of both genders" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Ze / Hir", Description = "Alternate pronouns that are gender neutral. Pronounced /zee/ and /here/ they replace “he” and “she” and “his” and “hers” respectively. Alternatively some people who are not comfortable/do not embrace he/she use the plural pronoun “they/their” as a gender neutral singular pronoun" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Non-Binary", Description = "Someone who does not identify as a man or a woman, or solely as one of those two genders. It’s often used as an umbrella term for identities that fall outside the male/female gender binary. Being non-binary means different things to different people, so this definition is purposely broad" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Gender Neutral", Description = "Someone who feels they are neither male or female may identify as gender neutral" });

        DataUtilities.SetActiveFieldToTrue(relationalRecords);
        DataUtilities.SetAuditFields(relationalRecords);

        var ret = new CommonRelationalData { ReferenceType = CommonRelationalDataReferenceTypes.PersonGender, Description = "Gender for a person / contact / user", Json = JsonSerializer.Serialize(relationalRecords) };
        return ret;
    }

    private CommonRelationalData _configurePersonMaritalStatus(EntityTypeBuilder<CommonRelationalData> builder)
    {
        var relationalRecords = new List<CommonRelationalDataDto>();
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Agender", Description = "Not having a gender or identifying with a gender. They may describe themselves as being gender neutral or genderless" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Married (And Not Separated)", Description = "This category includes persons whose opposite- or same-sex spouse is living, unless the couple is separated or a divorce has been obtained. Also included are persons in civil unions." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Widowed (Including Living Common Law)", Description = "This category includes persons who have lost their legally-married spouse through death and have not remarried. Those who live with a common-law partner are included in this category." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Separated (Including Living Common Law)", Description = "This category includes persons currently legally married but who are no longer living with their spouse (for any reason other than illness, work or school) and have not obtained a divorce. Those who live with a common-law partner are included in this category." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Divorced (Including Living Common Law)", Description = "This category includes persons who have obtained a legal divorce and have not remarried. Those who live with a common-law partner are included in this category." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Single (Including Living Common Law)", Description = "This category includes persons who have never married (including all persons less than 15 years of age). It also includes persons whose marriage has been legally annulled who were single before the annulled marriage and who have not remarried. Those who live with a common-law partner are included in this category." });

        DataUtilities.SetActiveFieldToTrue(relationalRecords);
        DataUtilities.SetAuditFields(relationalRecords);

        var ret = new CommonRelationalData { ReferenceType = CommonRelationalDataReferenceTypes.PersonMaritalStatus, Description = "Marital status for a person / contact / user", Json = JsonSerializer.Serialize(relationalRecords) };
        return ret;
    }

    private CommonRelationalData _configurePersonReligion(EntityTypeBuilder<CommonRelationalData> builder)
    {
        var relationalRecords = new List<CommonRelationalDataDto>();
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Agender", Description = "Not having a gender or identifying with a gender. They may describe themselves as being gender neutral or genderless" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Christian" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Muslim" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Hindu" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Agnostic" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Buddhist" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Chinese Universist" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Ethnoreligionist" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Atheist" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Neoreligionist" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Sikh" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Jew" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Spiritist" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Taoist" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Confucianist" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Bahai" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Jain" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Shinto" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Zoroastrian" });

        DataUtilities.SetActiveFieldToTrue(relationalRecords);
        DataUtilities.SetAuditFields(relationalRecords);

        var ret = new CommonRelationalData { ReferenceType = CommonRelationalDataReferenceTypes.PersonReligion, Description = "Religion / Faith for a person / contact / user", Json = JsonSerializer.Serialize(relationalRecords) };
        return ret;
    }

    private CommonRelationalData _configurePersonSexuality(EntityTypeBuilder<CommonRelationalData> builder)
    {
        var relationalRecords = new List<CommonRelationalDataDto>();
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Agender", Description = "Not having a gender or identifying with a gender. They may describe themselves as being gender neutral or genderless" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Abrosexual", Description = "Abrosexuality is described as an individual who experiences their sexuality change frequently. It can fluctuate between different sexualities often" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Androgynosexual", Description = "An androgynosexual describes a person who is sexually attracted to men and women, particularly those of androgynous appearance" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Androsexual", Description = "An androsexual is anyone who has sexual feelings towards masculinity. This term is usually used by Sexuality-queer individuals who do not fall within heterosexuality or homosexuality" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Aromantic ", Description = "A person who is aromantic does not experience romantic attraction. This person does not have to identify as asexual & they still may experience sensual & aesthetic attraction" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Asexual ", Description = "Asexual is the term used to describe a person who feels little to no sexual attraction to anyone" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Biromantic ", Description = "A person who is romantically attracted to two sexes or Sexualitys. Biromantic asexuals seek romantic relationships for companionship, affection, and intimacy, but they are not sexually attracted to their romantic partners" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Bisexuality ", Description = "Bisexuals are sexually attracted to two or more Sexualitys" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Ceterosexual ", Description = "Someone who experiences sexual/romantic attraction only to non-binary people. This label is used as a non-problematic term for skoliosexuality only to be used by non-binary people" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Demisexual ", Description = "Demisexual refers to a person who doesn’t experience sexual attraction unless they form an emotional connection" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Demiromantic ", Description = "This term is a type of grey-romantic who only experiences romantic attraction after developing an emotional connection. They do not experience primary romantic attraction, but are capable of secondary romantic attraction" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Finsexual ", Description = "The attraction to women, females and femininity " });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Gay", Description = "A person who identifies as homosexual" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Gynosexual ", Description = "Anyone who has sexual feelings towards a woman or femininity. The term can be useful when describing the sexual orientation of an individual with a non-binary Sexuality identity" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Grey-Romantic", Description = "A grey-romantic is a person with a romantic orientation that is somewhere between aromantic & romantic" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Heterosexual ", Description = "A person sexually attracted to people of the opposite sex" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Homosexuality ", Description = "A person sexually attracted to people of ones own sex" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Heteroromantic ", Description = "Romantically attracted to a member of the opposite sex or Sexuality. They may seek romantic intimacy but they’re not sexually attracted to partners" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Homoromantic ", Description = "Romantically attracted to someone of the same sex or Sexuality. They may seek romantic intimacy but they’re not sexually attracted to partners" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Lesbian", Description = "A homosexual woman whose emotional, romantic and sexual feelings are towards women" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Omnisexual", Description = "Omnisexuals are attracted to all genders,=. Gender can still be a factor in their attraction, unlike pansexuals, who don’t care about gender" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Pansexuality", Description = "A person whose romantic and/or sexual attraction towards others is not limited by sex or gender" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Panromantic", Description = "Is a person that can be romantically attracted to all genders, but not sexually" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Pomosexuality", Description = "Refers to non-orientation in which people disregard sexuality labels altogether. Basically, labels are seen as superficial and insignificant to someone who identities as pomosexual" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Polysexual", Description = "Is the attract to some, but not all, genders. They may be attracted to any combination of  possible genders, including binary & non-binary" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Queer", Description = "An LGBTQ+ term the community is reclaiming. An identity label that is non-specific about a persons sexual orientation" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Questioning", Description = "To be unsure of or re-examining ones previous assumption of sexual orientation" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Straight", Description = "An heterosexual" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Skoliosexual", Description = "The attraction to non-binary individuals" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Sapiosexual", Description = "Sexually attracted to intelligence or the human mind" });

        DataUtilities.SetActiveFieldToTrue(relationalRecords);
        DataUtilities.SetAuditFields(relationalRecords);

        var ret = new CommonRelationalData { ReferenceType = CommonRelationalDataReferenceTypes.PersonSexuality, Description = "Sexuality for a person / contact / user", Json = JsonSerializer.Serialize(relationalRecords) };
        return ret;
    }

    private CommonRelationalData _configurePersonLanguage(EntityTypeBuilder<CommonRelationalData> builder)
    {
        var relationalRecords = new List<CommonRelationalDataDto>();
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Afrikaans", Description = "A West Germanic language spoken mainly in South Africa and Namibia." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Arabic", Description = "A Semitic language with many dialects, widely spoken across the Middle East and North Africa." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Bengali", Description = "An Indo-Aryan language spoken primarily in Bangladesh and eastern India." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Chinese (Mandarin)", Description = "The most spoken language in the world, official in China and Taiwan." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Dutch", Description = "A West Germanic language spoken mainly in the Netherlands and Belgium." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "English", Description = "A global lingua franca, native to the UK, USA, and many other countries." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "French", Description = "A Romance language spoken in France, Canada, Africa, and other regions." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "German", Description = "A West Germanic language spoken in Germany, Austria, and Switzerland." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Hindi", Description = "An Indo-Aryan language widely spoken in India and parts of Nepal." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Italian", Description = "A Romance language spoken mainly in Italy and parts of Switzerland." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Japanese", Description = "An East Asian language spoken primarily in Japan." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Korean", Description = "An East Asian language spoken in both South and North Korea." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Portuguese", Description = "A Romance language spoken in Portugal, Brazil, and parts of Africa." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Russian", Description = "An East Slavic language spoken in Russia and many former Soviet states." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Spanish", Description = "A Romance language spoken in Spain, Latin America, and parts of the USA." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Swedish", Description = "A North Germanic language spoken mainly in Sweden and Finland." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Thai", Description = "The official language of Thailand, part of the Tai-Kadai family." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Turkish", Description = "A Turkic language spoken mainly in Turkey and Cyprus." });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Vietnamese", Description = "The official language of Vietnam, part of the Austroasiatic family." });
        
        DataUtilities.SetActiveFieldToTrue(relationalRecords);
        DataUtilities.SetAuditFields(relationalRecords);

        var ret = new CommonRelationalData { ReferenceType = CommonRelationalDataReferenceTypes.PersonLanguage, Description = "Language for a person / contact / user", Json = JsonSerializer.Serialize(relationalRecords) };
        return ret;
    }

    #endregion

    #region Address / Location

    private CommonRelationalData _configureAddressType(EntityTypeBuilder<CommonRelationalData> builder)
    {
        var relationalRecords = new List<CommonRelationalDataDto>();
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Home" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Work" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "School" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Other" });

        DataUtilities.SetActiveFieldToTrue(relationalRecords);
        DataUtilities.SetAuditFields(relationalRecords);

        var ret = new CommonRelationalData { ReferenceType = CommonRelationalDataReferenceTypes.AddressType, Description = "Address type for a person / contact / user", Json = JsonSerializer.Serialize(relationalRecords) };
        return ret;
    }

    private CommonRelationalData _configurePhoneNumberType(EntityTypeBuilder<CommonRelationalData> builder)
    {
        var relationalRecords = new List<CommonRelationalDataDto>();
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Home" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Mobile" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Home" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Work" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "School" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "iPhone" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Apple Watch" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Main" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Home Fax" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Work Fax" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Pager" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Other" });

        DataUtilities.SetActiveFieldToTrue(relationalRecords);
        DataUtilities.SetAuditFields(relationalRecords);

        var ret = new CommonRelationalData { ReferenceType = CommonRelationalDataReferenceTypes.PhoneNumberType, Description = "Phone number type for a person / contact / user", Json = JsonSerializer.Serialize(relationalRecords) };
        return ret;
    }

    private CommonRelationalData _configureUsaState(EntityTypeBuilder<CommonRelationalData> builder)
    {
        var relationalRecords = new List<CommonRelationalDataDto>();
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Alabama", Value = "AL" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Alaska", Value = "AK" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Arizona", Value = "AZ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Arkansas", Value = "AR" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "California", Value = "CA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Colorado", Value = "CO" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Connecticut", Value = "CT" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Delaware", Value = "DE" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Florida", Value = "FL" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Georgia", Value = "GA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Hawaii", Value = "HI" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Idaho", Value = "ID" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Illinois", Value = "IL" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Indiana", Value = "IN" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Iowa", Value = "IA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Kansas", Value = "KS" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Kentucky", Value = "KY" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Louisiana", Value = "LA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Maine", Value = "ME" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Maryland", Value = "MD" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Massachusetts", Value = "MA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Michigan", Value = "MI" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Minnesota", Value = "MN" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Mississippi", Value = "MS" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Missouri", Value = "MO" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Montana", Value = "MT" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Nebraska", Value = "NE" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Nevada", Value = "NV" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "New Hampshire", Value = "NH" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "New Jersey", Value = "NJ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "New Mexico", Value = "NM" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "New York", Value = "NY" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "North Carolina", Value = "NC" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "North Dakota", Value = "ND" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Ohio", Value = "OH" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Oklahoma", Value = "OK" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Oregon", Value = "OR" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Pennsylvania", Value = "PA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Rhode Island", Value = "RI" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "South Carolina", Value = "SC" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "South Dakota", Value = "SD" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Tennessee", Value = "TN" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Texas", Value = "TX" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Utah", Value = "UT" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Vermont", Value = "VT" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Virginia", Value = "VA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Washington", Value = "WA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "West Virginia", Value = "WV" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Wisconsin", Value = "WI" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Wyoming", Value = "WY" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "American Samoa", Value = "AS" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "District Of Colombia", Value = "DC" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Federated States of Micronesia", Value = "FM" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Guam", Value = "GU" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Marshall Islands", Value = "MH" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Northern Mariana Islands", Value = "MP" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Palau", Value = "PW" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Puerto Rico", Value = "PR" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Virgin Islands", Value = "VI" });

        DataUtilities.SetActiveFieldToTrue(relationalRecords);
        DataUtilities.SetAuditFields(relationalRecords);

        var ret = new CommonRelationalData { ReferenceType = CommonRelationalDataReferenceTypes.UsaState, Description = "List of all Usa States and Territories", Json = JsonSerializer.Serialize(relationalRecords) };
        return ret;
    }

    private CommonRelationalData _configureCountry(EntityTypeBuilder<CommonRelationalData> builder)
    {
        var relationalRecords = new List<CommonRelationalDataDto>();
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Alabama", Value = "AL" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Afghanistan", Value = "AF" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Albania", Value = "AL" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Algeria", Value = "DZ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Aland Islands", Value = "AX" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "American Samoa", Value = "AS" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Anguilla", Value = "AI" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Andorra", Value = "AD" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Angola", Value = "AO" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Antilles - Netherlands", Value = "AN" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Antigua and Barbuda", Value = "AG" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Antarctica", Value = "AQ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Argentina", Value = "AR" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Armenia", Value = "AM" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Australia", Value = "AU" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Austria", Value = "AT" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Aruba", Value = "AW" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Azerbaijan", Value = "AZ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Bosnia and Herzegovina", Value = "BA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Barbados", Value = "BB" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Bangladesh", Value = "BD" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Belgium", Value = "BE" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Burkina Faso", Value = "BF" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Bulgaria", Value = "BG" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Bahrain", Value = "BH" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Burundi", Value = "BI" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Benin", Value = "BJ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Bermuda", Value = "BM" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Brunei Darussalam", Value = "BN" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Bolivia", Value = "BO" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Brazil", Value = "BR" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Bahamas", Value = "BS" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Bhutan", Value = "BT" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Bouvet Island", Value = "BV" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Botswana", Value = "BW" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Belarus", Value = "BY" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Belize", Value = "BZ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Cambodia", Value = "KH" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Cameroon", Value = "CM" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Canada", Value = "CA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Cape Verde", Value = "CV" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Central African Republic", Value = "CF" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Chad", Value = "TD" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Chile", Value = "CL" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "China", Value = "CN" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Christmas Island", Value = "CX" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Cocos (Keeling) Islands", Value = "CC" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Colombia", Value = "CO" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Congo", Value = "CG" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Cote D'Ivoire (Ivory Coast)", Value = "CI" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Cook Islands", Value = "CK" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Costa Rica", Value = "CR" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Croatia (Hrvatska)", Value = "HR" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Cuba", Value = "CU" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Cyprus", Value = "CY" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Czech Republic", Value = "CZ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Democratic Republic of the Congo", Value = "CD" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Djibouti", Value = "DJ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Denmark", Value = "DK" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Dominica", Value = "DM" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Dominican Republic", Value = "DO" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Ecuador", Value = "EC" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Egypt", Value = "EG" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "El Salvador", Value = "SV" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "East Timor", Value = "TP" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Estonia", Value = "EE" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Equatorial Guinea", Value = "GQ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Eritrea", Value = "ER" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Ethiopia", Value = "ET" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Finland", Value = "FI" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Fiji", Value = "FJ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Falkland Islands (Malvinas)", Value = "FK" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Federated States of Micronesia", Value = "FM" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Faroe Islands", Value = "FO" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "France", Value = "FR" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "France, Metropolitan", Value = "FX" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "French Guiana", Value = "GF" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "French Polynesia", Value = "PF" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Gabon", Value = "GA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Gambia", Value = "GM" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Germany", Value = "DE" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Ghana", Value = "GH" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Gibraltar", Value = "GI" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Great Britain (UK)", Value = "GB" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Grenada", Value = "GD" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Georgia", Value = "GE" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Greece", Value = "GR" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Greenland", Value = "GL" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Guinea", Value = "GN" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Guadeloupe", Value = "GP" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "S. Georgia and S. Sandwich Islands", Value = "GS" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Guatemala", Value = "GT" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Guam", Value = "GU" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Guinea-Bissau", Value = "GW" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Guyana", Value = "GY" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Hong Kong", Value = "HK" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Heard Island and McDonald Islands", Value = "HM" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Honduras", Value = "HN" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Haiti", Value = "HT" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Hungary", Value = "HU" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Indonesia", Value = "ID" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Ireland", Value = "IE" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Israel", Value = "IL" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "India", Value = "IN" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "British Indian Ocean Territory", Value = "IO" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Iraq", Value = "IQ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Iran", Value = "IR" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Italy", Value = "IT" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Jamaica", Value = "JM" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Jordan", Value = "JO" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Japan", Value = "JP" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Kenya", Value = "KE" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Kyrgyzstan", Value = "KG" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Kiribati", Value = "KI" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Comoros", Value = "KM" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Saint Kitts and Nevis", Value = "KN" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "North Korea", Value = "KP" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "South Korea", Value = "KR" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Kuwait", Value = "KW" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Cayman Islands", Value = "KY" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Kazakhstan", Value = "KZ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Laos", Value = "LA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Lebanon", Value = "LB" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Saint Lucia", Value = "LC" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Liechtenstein", Value = "LI" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Sri Lanka", Value = "LK" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Liberia", Value = "LR" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Lesotho", Value = "LS" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Lithuania", Value = "LT" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Luxembourg", Value = "LU" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Latvia", Value = "LV" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Libya", Value = "LY" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Macedonia", Value = "MK" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Macao", Value = "MO" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Madagascar", Value = "MG" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Malaysia", Value = "MY" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Mali", Value = "ML" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Malawi", Value = "MW" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Mauritania", Value = "MR" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Marshall Islands", Value = "MH" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Martinique", Value = "MQ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Mauritius", Value = "MU" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Mayotte", Value = "YT" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Malta", Value = "MT" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Mexico", Value = "MX" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Morocco", Value = "MA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Monaco", Value = "MC" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Moldova", Value = "MD" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Mongolia", Value = "MN" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Myanmar", Value = "MM" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Northern Mariana Islands", Value = "MP" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Montserrat", Value = "MS" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Maldives", Value = "MV" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Mozambique", Value = "MZ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Namibia", Value = "NA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "New Caledonia", Value = "NC" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Niger", Value = "NE" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Norfolk Island", Value = "NF" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Nigeria", Value = "NG" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Nicaragua", Value = "NI" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Netherlands", Value = "NL" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Norway", Value = "NO" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Nepal", Value = "NP" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Nauru", Value = "NR" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Niue", Value = "NU" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "New Zealand (Aotearoa)", Value = "NZ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Oman", Value = "OM" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Panama", Value = "PA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Peru", Value = "PE" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Papua New Guinea", Value = "PG" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Philippines", Value = "PH" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Pakistan", Value = "PK" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Poland", Value = "PL" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Saint Pierre and Miquelon", Value = "PM" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Serbia and Montenegro", Value = "CS" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Pitcairn", Value = "PN" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Puerto Rico", Value = "PR" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Palestinian Territory", Value = "PS" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Portugal", Value = "PT" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Palau", Value = "PW" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Paraguay", Value = "PY" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Qatar", Value = "QA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Reunion", Value = "RE" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Romania", Value = "RO" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Russian Federation", Value = "RU" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Rwanda", Value = "RW" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Saudi Arabia", Value = "SA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Samoa", Value = "WS" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Saint Helena", Value = "SH" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Saint Vincent and the Grenadines", Value = "VC" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "San Marino", Value = "SM" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Sao Tome and Principe", Value = "ST" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Senegal", Value = "SN" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Seychelles", Value = "SC" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Sierra Leone", Value = "SL" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Singapore", Value = "SG" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Slovakia", Value = "SK" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Slovenia", Value = "SI" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Solomon Islands", Value = "SB" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Somalia", Value = "SO" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "South Africa", Value = "ZA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Spain", Value = "ES" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Sudan", Value = "SD" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Suriname", Value = "SR" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Svalbard and Jan Mayen", Value = "SJ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Sweden", Value = "SE" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Switzerland", Value = "CH" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Syria", Value = "SY" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "USSR (former)", Value = "SU" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Swaziland", Value = "SZ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Taiwan", Value = "TW" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Tanzania", Value = "TZ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Tajikistan", Value = "TJ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Thailand", Value = "TH" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Timor-Leste", Value = "TL" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Togo", Value = "TG" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Tokelau", Value = "TK" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Tonga", Value = "TO" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Trinidad and Tobago", Value = "TT" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Tunisia", Value = "TN" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Turkey", Value = "TR" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Turkmenistan", Value = "TM" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Turks and Caicos Islands", Value = "TC" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Tuvalu", Value = "TV" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Ukraine", Value = "UA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Uganda", Value = "UG" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "United Arab Emirates", Value = "AE" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "United Kingdom", Value = "UK" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "United States", Value = "US" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "United States Minor Outlying Islands", Value = "UM" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Uruguay", Value = "UY" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Uzbekistan", Value = "UZ" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Vanuatu", Value = "VU" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Vatican City State", Value = "VA" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Venezuela", Value = "VE" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Virgin Islands (British)", Value = "VG" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Virgin Islands (U.S.)", Value = "VI" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Viet Nam", Value = "VN" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Wallis and Futuna", Value = "WF" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Western Sahara", Value = "EH" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Yemen", Value = "YE" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Yugoslavia (former)", Value = "YU" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Zambia", Value = "ZM" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Zaire (former)", Value = "ZR" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Zimbabwe", Value = "ZW" });

        DataUtilities.SetActiveFieldToTrue(relationalRecords);
        DataUtilities.SetAuditFields(relationalRecords);

        var ret = new CommonRelationalData { ReferenceType = CommonRelationalDataReferenceTypes.Country, Description = "List of all Countries and their Value", Json = JsonSerializer.Serialize(relationalRecords) };

        return ret;
    }

    private CommonRelationalData _configureTimeZone(EntityTypeBuilder<CommonRelationalData> builder)
    {
        var relationalRecords = new List<CommonRelationalDataDto>();
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Eastern Standard Time (EST):", Value = "EST", Description = "Covers the East Coast and parts of the Midwest" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Central Standard Time (CST):", Value = "CST", Description = "Spans the central US and Gulf Coast" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Mountain Standard Time (MST):", Value = "MST", Description = "Extends across the Mountain West" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Pacific Standard Time (PST):", Value = "PST", Description = "Covers the West Coast, including California, and Nevada" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Alaska Standard Time (AKST):", Value = "AKST", Description = "Covers almost the entire state of Alaska" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Hawaii-Aleutian Standard Time (HST)", Value = "HST", Description = "Covers Hawaii and parts of the Aleutian Islands in Alaska" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Atlantic Standard Time (AST)", Value = "AST", Description = "Puerto Rico and the US Virgin Islands" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Samoa Standard Time (SST)", Value = "SST", Description = "American Samoa" });
        relationalRecords.Add(new CommonRelationalDataDto { Name = "Chamorro Standard Time (CHST)", Value = "CHST", Description = "Guam and the Northern Mariana Islands" });

        DataUtilities.SetActiveFieldToTrue(relationalRecords);
        DataUtilities.SetAuditFields(relationalRecords);

        var ret = new CommonRelationalData { ReferenceType = CommonRelationalDataReferenceTypes.UsaTimeZone, Description = "List of all USA Time Zones and their Value", Json = JsonSerializer.Serialize(relationalRecords) };

        return ret;
    }

    #endregion
    
}