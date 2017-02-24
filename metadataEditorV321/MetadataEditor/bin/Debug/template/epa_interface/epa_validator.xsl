<?xml version="1.0" standalone="yes"?>
<axsl:stylesheet xmlns:axsl="http://www.w3.org/1999/XSL/Transform" xmlns:sch="http://www.ascc.net/xml/schematron" xmlns:iso="http://purl.oclc.org/dsdl/schematron" version="1.0">
	<!--Implementers: please note that overriding process-prolog or process-root is 
    the preferred method for meta-stylesheets to use where possible. The name or details of 
    this mode may change during 1Q 2007.-->


	<!--PHASES-->


	<!--PROLOG-->
	<axsl:output method="xml" omit-xml-declaration="no" standalone="yes" indent="yes"/>

	<!--KEYS-->


	<!--DEFAULT RULES-->


	<!--MODE: SCHEMATRON-FULL-PATH-->
	<!--This mode can be used to generate an ugly though full XPath for locators-->
	<axsl:template match="*" mode="schematron-get-full-path">
		<axsl:apply-templates select="parent::*" mode="schematron-get-full-path-2"/>
		<axsl:text>/</axsl:text>
		<axsl:choose>
			<axsl:when test="namespace-uri()=''">
				<axsl:value-of select="name()"/>
			</axsl:when>
			<axsl:otherwise>
				<axsl:text>*:</axsl:text>
				<axsl:value-of select="local-name()"/>
				<axsl:text>[namespace-uri()='</axsl:text>
				<axsl:value-of select="namespace-uri()"/>
				<axsl:text>']</axsl:text>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:variable name="preceding" select="count(preceding-sibling::*[local-name()=local-name(current())                                   and namespace-uri() = namespace-uri(current())])"/>
		<axsl:text>[</axsl:text>
		<axsl:value-of select="1+ $preceding"/>
		<axsl:text>]</axsl:text>
	</axsl:template>
	<axsl:template match="@*" mode="schematron-get-full-path">
		<axsl:apply-templates select="parent::*" mode="schematron-get-full-path"/>
		<axsl:text>/</axsl:text>
		<axsl:choose>
			<axsl:when test="namespace-uri()=''">@iso:schema</axsl:when>
			<axsl:otherwise>
				<axsl:text>@*[local-name()='</axsl:text>
				<axsl:value-of select="local-name()"/>
				<axsl:text>' and namespace-uri()='</axsl:text>
				<axsl:value-of select="namespace-uri()"/>
				<axsl:text>']</axsl:text>
			</axsl:otherwise>
		</axsl:choose>
	</axsl:template>

	<!--MODE: SCHEMATRON-FULL-PATH-2-->
	<!--This mode can be used to generate prefixed XPath for humans-->
	<axsl:template match="node() | @*" mode="schematron-get-full-path-2">
		<axsl:for-each select="ancestor-or-self::*">
			<axsl:text>/</axsl:text>
			<axsl:value-of select="name(.)"/>
			<axsl:if test="preceding-sibling::*[name(.)=name(current())]">
				<axsl:text>[</axsl:text>
				<axsl:value-of select="count(preceding-sibling::*[name(.)=name(current())])+1"/>
				<axsl:text>]</axsl:text>
			</axsl:if>
		</axsl:for-each>
		<axsl:if test="not(self::*)">
			<axsl:text/>/@<axsl:value-of select="name(.)"/>
		</axsl:if>
	</axsl:template>

	<!--MODE: GENERATE-ID-FROM-PATH -->
	<axsl:template match="/" mode="generate-id-from-path"/>
	<axsl:template match="text()" mode="generate-id-from-path">
		<axsl:apply-templates select="parent::*" mode="generate-id-from-path"/>
		<axsl:value-of select="concat('.text-', 1+count(preceding-sibling::text()), '-')"/>
	</axsl:template>
	<axsl:template match="comment()" mode="generate-id-from-path">
		<axsl:apply-templates select="parent::*" mode="generate-id-from-path"/>
		<axsl:value-of select="concat('.comment-', 1+count(preceding-sibling::comment()), '-')"/>
	</axsl:template>
	<axsl:template match="processing-instruction()" mode="generate-id-from-path">
		<axsl:apply-templates select="parent::*" mode="generate-id-from-path"/>
		<axsl:value-of select="concat('.processing-instruction-', 1+count(preceding-sibling::processing-instruction()), '-')"/>
	</axsl:template>
	<axsl:template match="@*" mode="generate-id-from-path">
		<axsl:apply-templates select="parent::*" mode="generate-id-from-path"/>
		<axsl:value-of select="concat('.@', name())"/>
	</axsl:template>
	<axsl:template match="*" mode="generate-id-from-path" priority="-0.5">
		<axsl:apply-templates select="parent::*" mode="generate-id-from-path"/>
		<axsl:text>.</axsl:text>
		<axsl:choose>
			<axsl:when test="count(. | ../namespace::*) = count(../namespace::*)">
				<axsl:value-of select="concat('.namespace::-',1+count(namespace::*),'-')"/>
			</axsl:when>
			<axsl:otherwise>
				<axsl:value-of select="concat('.',name(),'-',1+count(preceding-sibling::*[name()=name(current())]),'-')"/>
			</axsl:otherwise>
		</axsl:choose>
	</axsl:template>

	<!--MODE: GENERATE-ID-2 -->
	<axsl:template match="/" mode="generate-id-2">U</axsl:template>
	<axsl:template match="*" mode="generate-id-2" priority="2">
		<axsl:text>U</axsl:text>
		<axsl:number level="multiple" count="*"/>
	</axsl:template>
	<axsl:template match="node()" mode="generate-id-2">
		<axsl:text>U.</axsl:text>
		<axsl:number level="multiple" count="*"/>
		<axsl:text>n</axsl:text>
		<axsl:number count="node()"/>
	</axsl:template>
	<axsl:template match="@*" mode="generate-id-2">
		<axsl:text>U.</axsl:text>
		<axsl:number level="multiple" count="*"/>
		<axsl:text>_</axsl:text>
		<axsl:value-of select="string-length(local-name(.))"/>
		<axsl:text>_</axsl:text>
		<axsl:value-of select="translate(name(),':','.')"/>
	</axsl:template>
	<!--Strip characters-->
	<axsl:template match="text()" priority="-1"/>

	<!--SCHEMA METADATA-->
	<axsl:template match="/">
		<svrl:schematron-output xmlns:svrl="http://purl.oclc.org/dsdl/svrl" title="EPA Validator ISO schematron file." schemaVersion="ISO19757-3">
			<svrl:active-pattern xmlns:sch="http://www.ascc.net/xml/schematron" xmlns:iso="http://purl.oclc.org/dsdl/schematron" name="dataqual_err" id="dataqual_err">
				<axsl:apply-templates/>
			</svrl:active-pattern>
			<axsl:apply-templates select="/" mode="M1"/>
			<svrl:active-pattern xmlns:sch="http://www.ascc.net/xml/schematron" xmlns:iso="http://purl.oclc.org/dsdl/schematron" name="dataqual_glob" id="dataqual_glob">
				<axsl:apply-templates/>
			</svrl:active-pattern>
			<axsl:apply-templates select="/" mode="M2"/>
			<svrl:active-pattern xmlns:sch="http://www.ascc.net/xml/schematron" xmlns:iso="http://purl.oclc.org/dsdl/schematron" name="distinfo_err" id="distinfo_err">
				<axsl:apply-templates/>
			</svrl:active-pattern>
			<axsl:apply-templates select="/" mode="M3"/>
			<svrl:active-pattern xmlns:sch="http://www.ascc.net/xml/schematron" xmlns:iso="http://purl.oclc.org/dsdl/schematron" name="distinfo_glob" id="distinfo_glob">
				<axsl:apply-templates/>
			</svrl:active-pattern>
			<axsl:apply-templates select="/" mode="M4"/>
			<svrl:active-pattern xmlns:sch="http://www.ascc.net/xml/schematron" xmlns:iso="http://purl.oclc.org/dsdl/schematron" name="idinfo_err" id="idinfo_err">
				<axsl:apply-templates/>
			</svrl:active-pattern>
			<axsl:apply-templates select="/" mode="M5"/>
			<svrl:active-pattern xmlns:sch="http://www.ascc.net/xml/schematron" xmlns:iso="http://purl.oclc.org/dsdl/schematron" name="idinfo_glob" id="idinfo_glob">
				<axsl:apply-templates/>
			</svrl:active-pattern>
			<axsl:apply-templates select="/" mode="M6"/>
			<svrl:active-pattern xmlns:sch="http://www.ascc.net/xml/schematron" xmlns:iso="http://purl.oclc.org/dsdl/schematron" name="metainfo_err" id="metainfo_err">
				<axsl:apply-templates/>
			</svrl:active-pattern>
			<axsl:apply-templates select="/" mode="M7"/>
			<svrl:active-pattern xmlns:sch="http://www.ascc.net/xml/schematron" xmlns:iso="http://purl.oclc.org/dsdl/schematron" name="metainfo_glob" id="metainfo_glob">
				<axsl:apply-templates/>
			</svrl:active-pattern>
			<axsl:apply-templates select="/" mode="M8"/>
			<svrl:active-pattern xmlns:sch="http://www.ascc.net/xml/schematron" xmlns:iso="http://purl.oclc.org/dsdl/schematron" name="req_sections" id="req_sections">
				<axsl:apply-templates/>
			</svrl:active-pattern>
			<axsl:apply-templates select="/" mode="M9"/>
			<svrl:active-pattern xmlns:sch="http://www.ascc.net/xml/schematron" xmlns:iso="http://purl.oclc.org/dsdl/schematron" name="ok_values_geodetic" id="ok_values_geodetic">
				<axsl:apply-templates/>
			</svrl:active-pattern>
			<axsl:apply-templates select="/" mode="M10"/>
			<svrl:active-pattern xmlns:sch="http://www.ascc.net/xml/schematron" xmlns:iso="http://purl.oclc.org/dsdl/schematron" name="spref_err" id="spref_err">
				<axsl:apply-templates/>
			</svrl:active-pattern>
			<axsl:apply-templates select="/" mode="M11"/>
			<svrl:active-pattern xmlns:sch="http://www.ascc.net/xml/schematron" xmlns:iso="http://purl.oclc.org/dsdl/schematron" name="spref_glob" id="spref_glob">
				<axsl:apply-templates/>
			</svrl:active-pattern>
			<axsl:apply-templates select="/" mode="M12"/>
		</svrl:schematron-output>
	</axsl:template>

	<!--SCHEMATRON PATTERNS-->
	<svrl:text xmlns:svrl="http://purl.oclc.org/dsdl/svrl">EPA Validator ISO schematron file.</svrl:text>

	<!--PATTERN dataqual_err-->


	<!--RULE -->
	<axsl:template match="dataqual/posacc/horizpa/horizpar" priority="4000" mode="M1">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="dataqual/posacc/horizpa/horizpar"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="normalize-space(.) != ''"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="normalize-space(.) != ''">
					<axsl:attribute name="location">/metadata/dataqual/posacc/horizpa/horizpar</axsl:attribute>
					<svrl:text>Error: EPA requires a non-empty 'horizpar' element: Horizontal Position Accuracy Report.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M1"/>
	</axsl:template>

	<!--RULE -->
	<axsl:template match="dataqual/posacc" priority="3999" mode="M1">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="dataqual/posacc"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="horizpa/horizpar"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="horizpa/horizpar">
					<axsl:attribute name="location">/metadata/dataqual/posacc/horizpa/horizpar</axsl:attribute>
					<svrl:text>Error: EPA requires a 'horizpar' element: Horizontal Position Accuracy Report.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M1"/>
	</axsl:template>

	<!--RULE -->
	<axsl:template match="dataqual/posacc//horizpav" priority="3998" mode="M1">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="dataqual/posacc//horizpav"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="translate(.,'0123456789.','')=''"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="translate(.,'0123456789.','')=''">
					<axsl:attribute name="location">/metadata/dataqual/posacc/horizpav</axsl:attribute>
					<svrl:text>Error: 'horizpav' must be a pure number (meters implied), saw '<axsl:text/><axsl:value-of select="//horizpav"/><axsl:text/>' element: Horizontal Postion Accuracy Value.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M1"/>
	</axsl:template>
	<axsl:template match="text()" priority="-1" mode="M1"/>
	<axsl:template match="@*|node()" priority="-2" mode="M1">
		<axsl:choose>
			<!--Housekeeping: SAXON warns if attempting to find the attribute
                           of an attribute-->
			<axsl:when test="not(@*)">
				<axsl:apply-templates select="node()" mode="M1"/>
			</axsl:when>
			<axsl:otherwise>
				<axsl:apply-templates select="@*|node()" mode="M1"/>
			</axsl:otherwise>
		</axsl:choose>
	</axsl:template>

	<!--PATTERN dataqual_glob-->


	<!--RULE -->
	<axsl:template match="dataqual" priority="4000" mode="M2">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="dataqual"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="//posacc"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="//posacc">
					<axsl:attribute name="location">/metadata/dataqual/posacc</axsl:attribute>
					<svrl:text>Error: EPA requires the 'posacc' element: positional information.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M2"/>
	</axsl:template>
	<axsl:template match="text()" priority="-1" mode="M2"/>
	<axsl:template match="@*|node()" priority="-2" mode="M2">
		<axsl:choose>
			<!--Housekeeping: SAXON warns if attempting to find the attribute
                           of an attribute-->
			<axsl:when test="not(@*)">
				<axsl:apply-templates select="node()" mode="M2"/>
			</axsl:when>
			<axsl:otherwise>
				<axsl:apply-templates select="@*|node()" mode="M2"/>
			</axsl:otherwise>
		</axsl:choose>
	</axsl:template>

	<!--PATTERN distinfo_err-->


	<!--RULE -->
	<axsl:template match="distinfo/resdesc" priority="4000" mode="M3">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="distinfo/resdesc"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="normalize-space(.) !=''"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="normalize-space(.) !=''">
					<axsl:attribute name="location">/metadata/distinfo/resdesc</axsl:attribute>
					<svrl:text>Error: EPA requires a non-empty 'resdesc' element: Resource Description.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M3"/>
	</axsl:template>

	<!--RULE -->
	<axsl:template match="distinfo/distliab" priority="3999" mode="M3">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="distinfo/distliab"/>
		<axsl:variable name="norm_distliab" select="normalize-space(.)"/>
		<axsl:variable name="norm_lit" select="normalize-space('Although these data have been processed successfully on a computer system at the Environmental Protection Agency, no warranty expressed or implied is made regarding the accuracy or utility of the data on any other system or for general or scientific purposes, nor shall the act of distribution constitute any such warranty. It is also strongly recommended that careful attention be paid to the contents of the metadata file associated with these data to evaluate data set limitations, restrictions or intended use. The U.S. Environmental Protection Agency shall not be held liable for improper or incorrect use of the data described and/or contained herein.')"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="$norm_distliab"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="$norm_distliab">
					<axsl:attribute name="location">/metadata/distinfo/distliab</axsl:attribute>
					<svrl:text>EPA requires a 'distliab' element</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M3"/>
	</axsl:template>

	<!--RULE -->
	<axsl:template match="distinfo/resdesc" priority="3998" mode="M3">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="distinfo/resdesc"/>
		<axsl:variable name="cv_resdesc" select="document('all.cv')/cv/ul[@id='resdesc']/li"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="$cv_resdesc[. = current()] or substring-before(.,':')='Geographic Services'"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="$cv_resdesc[. = current()] or substring-before(.,':')='Geographic Services'">
					<axsl:attribute name="location">/metadata/distinfo/resdesc</axsl:attribute>
					<svrl:text>Error: saw '<axsl:text/><axsl:value-of select="."/><axsl:text/>' EPA requires resdesc to match ESRI options:
						'Live Data and Maps', 'Downloadable Data', 'Offline Data', 'Map Files', 'Static Map Images',' Other Documents', 'Applications','Clearinghouses','Geographic Activities' or 'Geographic Services: &lt; Marketplace Record Designation &gt;</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M3"/>
	</axsl:template>
	<axsl:template match="text()" priority="-1" mode="M3"/>
	<axsl:template match="@*|node()" priority="-2" mode="M3">
		<axsl:choose>
			<!--Housekeeping: SAXON warns if attempting to find the attribute
                           of an attribute-->
			<axsl:when test="not(@*)">
				<axsl:apply-templates select="node()" mode="M3"/>
			</axsl:when>
			<axsl:otherwise>
				<axsl:apply-templates select="@*|node()" mode="M3"/>
			</axsl:otherwise>
		</axsl:choose>
	</axsl:template>

	<!--PATTERN distinfo_glob-->


	<!--RULE -->
	<axsl:template match="distinfo" priority="4000" mode="M4">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="distinfo"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="//resdesc"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="//resdesc">
					<axsl:attribute name="location">/metadata/distinfo/resdesc</axsl:attribute>
					<svrl:text>Error: EPA requires the 'resdesc' element: ESRI-defined categories for resource description.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="//distliab"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="//distliab">
					<axsl:attribute name="location">/metadata/distinfo/distliab</axsl:attribute>
					<svrl:text>Error: EPA requires the 'distliab' element: Distribution liability.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M4"/>
	</axsl:template>
	
	<!--RULE For EMAIL DCAT -->
	<axsl:template match="distinfo/distrib/cntinfo" priority="3999" mode="M4">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="distinfo/distrib/cntinfo"/>		
		
		<!--ASSERT-->
		<axsl:choose>
			<axsl:when test="/metadata/distinfo/distrib/cntinfo/cntemail"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="/metadata/distinfo/distrib/cntinfo/cntemail">
					<axsl:attribute name="location">/metadata/distinfo/distrib/cntinfo/cntemail</axsl:attribute>
					<svrl:text>Error: EPA requires a contact email (cntemail) element to exist in the distribution info section per data.gov mandate.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M4"/>
	</axsl:template>
	
	<!--RULE -->
	<axsl:template match="distinfo/distrib/cntinfo/cntperp" priority="3998" mode="M4">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="distinfo/distrib/cntinfo/cntperp"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="cntorg"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="cntorg">
					<axsl:attribute name="location">/metadata/distinfo/distrib/cntinfo/cntperp/cntorg</axsl:attribute>
					<svrl:text>Error: EPA requires a contact organization (cntorg) element to exist in the distribution info section per data.gov mandate.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="cntper"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="cntper">
					<axsl:attribute name="location">/metadata/distinfo/distrib/cntinfo/cntperp/cntper</axsl:attribute>
					<svrl:text>Error: EPA requires a contact person (cntper) element to exist in the distribution info section per data.gov mandate.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M4"/>
	</axsl:template>	      

	<!--RULE -->
	<axsl:template match="distinfo/distrib/cntinfo/cntorgp" priority="3997" mode="M4">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="distinfo/distrib/cntinfo/cntorgp"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="cntorg"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="cntorg">
					<axsl:attribute name="location">/metadata/distinfo/distrib/cntinfo/cntorgp/cntorg</axsl:attribute>
					<svrl:text>Error: EPA requires a contact organization (cntorg) element to exist in the distribution info section per data.gov mandate</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="cntper"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="cntper">
					<axsl:attribute name="location">/metadata/distinfo/distrib/cntinfo/cntorgp/cntper</axsl:attribute>
					<svrl:text>Error: EPA requires a contact person (cntper) element to exist in the distribution info section per data.gov mandate.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M4"/>
	</axsl:template>
    
	<axsl:template match="text()" priority="-1" mode="M4"/>
	<axsl:template match="@*|node()" priority="-2" mode="M4">
		<axsl:choose>
			<!--Housekeeping: SAXON warns if attempting to find the attribute
                           of an attribute-->
			<axsl:when test="not(@*)">
				<axsl:apply-templates select="node()" mode="M4"/>
			</axsl:when>
			<axsl:otherwise>
				<axsl:apply-templates select="@*|node()" mode="M4"/>
			</axsl:otherwise>
		</axsl:choose>
	</axsl:template>

	<!--PATTERN idinfo_err-->


	<!--RULE -->
	<axsl:template match="citeinfo/pubinfo//pubplace" priority="4000" mode="M5">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="citeinfo/pubinfo//pubplace"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="normalize-space(.) != ''"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="normalize-space(.) != ''">
					<axsl:attribute name="location">/metadata/idinfo/citation/citeinfo/pubinfo/pubplace</axsl:attribute>
					<svrl:text>Error: EPA requires the 'pubplace' element to have content: Place of Publication.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M5"/>
	</axsl:template>

	<!--RULE -->
	<axsl:template match="citeinfo/pubinfo//publish" priority="3999" mode="M5">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="citeinfo/pubinfo//publish"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="normalize-space(.) != ''"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="normalize-space(.) != ''">
					<axsl:attribute name="location">/metadata/idinfo/citation/citeinfo/pubinfo/publish</axsl:attribute>
					<svrl:text>Error: EPA requires the 'publish' element to have content: Publisher.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M5"/>
	</axsl:template>

	<!--RULE -->
	<axsl:template match="citeinfo/pubinfo" priority="3998" mode="M5">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="citeinfo/pubinfo"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="//pubplace"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="//pubplace">
					<axsl:attribute name="location">/metadata/idinfo/citation/citeinfo/pubinfo/pubplace</axsl:attribute>
					<svrl:text>Error: EPA requires the 'pubplace' element to exist.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="//publish"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="//publish">
					<axsl:attribute name="location">/metadata/idinfo/citation/citeinfo/pubinfo/pubplace</axsl:attribute>
					<svrl:text>Error: EPA requires the 'publish' element to exist: Publisher.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M5"/>
	</axsl:template>
	
	<!--RULE -->
	<!--ASSERT -->
	<!-- AE: 20080822 - Taken out by user requests.-->
	
	<axsl:template match="citeinfo/onlink" priority="3997" mode="M5"><svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="citeinfo/onlink"/>

		<axsl:choose>
			<axsl:when test="(substring-before(.,'://') = 'http' or   substring-before(.,'://') = 'ftp' or    substring-before(.,'://') = 'https') "/>
				<axsl:otherwise>
					<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="(substring-before(.,'://') = 'http' or substring-before(.,'://') = 'ftp' or substring-before(.,'://') = 'https')">
						<axsl:attribute name="location"><axsl:apply-templates select="." mode="schematron-get-full-path-2"/></axsl:attribute>						
						<svrl:text>Error: EPA requires the 'onlink' element to start with 'http://' or 'ftp://' or 'https://' we saw <axsl:text/><axsl:value-of select="."/><axsl:text/>.</svrl:text>
					</svrl:failed-assert>
				</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M5"/>
	</axsl:template>


	<!--RULE -->
	<axsl:template match="idinfo/keywords/theme[themekt='ISO 19115 Topic Category']/themekey" priority="3996" mode="M5">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="idinfo/keywords/theme[themekt='ISO 19115 Topic Category']/themekey"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="normalize-space(.) !=''"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="normalize-space(.) !=''">
					<axsl:attribute name="location">/metadata/idinfo/keywords/theme[themekt='ISO 19115 Topic Category']/themekt</axsl:attribute>
					<svrl:text>Error: EPA requires non-empty themekey in 'ISO 19115 Topic Category'</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M5"/>
	</axsl:template>

	<!--RULE -->
	<axsl:template match="idinfo/keywords" priority="3995" mode="M5">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="idinfo/keywords"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="theme[themekt='ISO 19115 Topic Category']"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="theme[themekt='ISO 19115 Topic Category']">
					<axsl:attribute name="location">/metadata/idinfo/keywords/theme[themekt='ISO 19115 Topic Category']/themekt</axsl:attribute>
					<svrl:text>Error: EPA requires an ISO 19115 Topic Category theme thesaurus, possible typo was '<axsl:text/><axsl:value-of select="theme[1]/themekt[1]"/><axsl:text/>'.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M5"/>
	</axsl:template>

	<!--RULE -->
	<axsl:template match="idinfo/keywords/theme[themekt='ISO 19115 Topic Category']" priority="3994" mode="M5">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="idinfo/keywords/theme[themekt='ISO 19115 Topic Category']"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="themekey"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="themekey">
					<axsl:attribute name="location">/metadata/idinfo/keywords/theme[themekt='ISO 19115 Topic Category']/themekey</axsl:attribute>
					<svrl:text>Error: EPA requires a themekey in 'ISO 19115 Topic Category' thesaurus.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M5"/>
	</axsl:template>

	<!--RULE -->
	<axsl:template match="idinfo/keywords/theme[themekt='ISO 19115 Topic Category']/themekey" priority="3993" mode="M5">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="idinfo/keywords/theme[themekt='ISO 19115 Topic Category']/themekey"/>
		<axsl:variable name="cv_geo1" select="document('all.cv')/cv/ul[@id='geo1']/li"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="$cv_geo1[. = current()]"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="$cv_geo1[. = current()]">
					<axsl:attribute name="location">/metadata/idinfo/keywords/theme[themekt='ISO 19115 Topic Category']/themekey</axsl:attribute>
					<svrl:text>Error: saw '<axsl:text/><axsl:value-of select="."/><axsl:text/>' EPA requires ISO 19115 Topic Category theme keywords to be chosen from
						'farming', 'biota', 'boundaries', 'climatologyMeteorologyAtmosphere', 'economy',
						'elevation', 'environment', 'geoscientificInformation', 'health', 'imageryBaseMapsEarthCover',
						'intelligenceMilitary', 'inlandWaters', 'location', 'oceans', 'planningCadastre', 'society',
						'structure', 'transportation' or 'utilitiesCommunication'.
					</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M5"/>
	</axsl:template>

	<!--RULE -->
	<axsl:template match="idinfo/keywords/theme[themekt='EPA GIS Keyword Thesaurus']/themekey" priority="3992" mode="M5">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="idinfo/keywords/theme[themekt='EPA GIS Keyword Thesaurus']/themekey"/>
		<axsl:variable name="cv_epa" select="document('all.cv')/cv/ul[@id='epa']/li"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="$cv_epa[. = current()]"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="$cv_epa[. = current()]">
					<axsl:attribute name="location">/metadata/idinfo/keywords/theme[themekt='EPA GIS Keyword Thesaurus']/themekey</axsl:attribute>
					<svrl:text>Error: saw '<axsl:text/><axsl:value-of select="."/><axsl:text/>' EPA requires EPA Geospatial Keyword Thesaurus keywords to be chosen from
						'Management', 'Disaster','Emergency', 'Energy','Environment', 'Monitoring', 'Air ',
						'Water', 'Land ', 'Biology','Ecosystem', 'Remediation', 'Cleanup',
						'Contaminant', 'Spills', 'Response','Hazards', 'Waste', 'Pesticides',
						'Toxics', 'Compliance', 'Impact ','Indicator', 'Risk', 'Exposure',
						'Modeling ', 'Quality', 'Indoor','Radiation', 'Climate ', 'Surface Water',
						'Ground Water', 'Marine ', 'Estuary ','Drinking Water', 'Health', 'Human',
						'Natural Resources', 'Conservation ', 'Marine','Disaster', 'Land', 'Recreation',
						'Agriculture', 'Ecology', 'Transportation','Ground ', 'Air ', 'Water',
						'Regulatory ', 'Compliance', 'Inspections',
						'Permits', 'Facilities' or 'Sites'
						.
					</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M5"/>
	</axsl:template>

	<!--RULE -->
	<axsl:template match="idinfo/secinfo" priority="3991" mode="M5">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="idinfo/secinfo"/>

		<!--ASSERT -->
        <!--no longer relevant for new use of secsys
		<axsl:choose>
			<axsl:when test="secsys = 'FIPS Pub 199' "/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="secsys = 'FIPS Pub 199'">
					<axsl:attribute name="location">/metadata/idinfo/secinfo/secsys</axsl:attribute>
					<svrl:text>Error: EPA requires the 'secsys' element to be 'FIPS Pub 199': saw '<axsl:text/><axsl:value-of select="secsys"/><axsl:text/>'.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
        -->

		<!--ASSERT DS -->
		<axsl:choose>
			<axsl:when test="secclass = 'public' or secclass = 'restricted public' or secclass='non-public' "/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="secclass = 'public' or secclass = 'non-public' or secclass='restricted public'">
					<axsl:attribute name="location">/metadata/idinfo/secinfo/secclass</axsl:attribute>
					<svrl:text>Error: EPA requires the 'secclass' element to be either 'public', 'restricted public' or 'non-public' per data.gov mandate (http://project-open-data.github.io/schema/#accessLevel): saw '<axsl:text/><axsl:value-of select="secclass"/><axsl:text/>'.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M5"/>
	</axsl:template>
	<axsl:template match="text()" priority="-1" mode="M5"/>
	<axsl:template match="@*|node()" priority="-2" mode="M5">
		<axsl:choose>
			<!--Housekeeping: SAXON warns if attempting to find the attribute
                           of an attribute-->
			<axsl:when test="not(@*)">
				<axsl:apply-templates select="node()" mode="M5"/>
			</axsl:when>
			<axsl:otherwise>
				<axsl:apply-templates select="@*|node()" mode="M5"/>
			</axsl:otherwise>
		</axsl:choose>
	</axsl:template>

	<!--PATTERN idinfo_glob-->


	<!--RULE -->
	<axsl:template match="idinfo" priority="4000" mode="M6">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="idinfo"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="//pubinfo"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="//pubinfo">
					<axsl:attribute name="location">/metadata/idinfo/citation/citeinfo/pubinfo</axsl:attribute>
					<svrl:text>Error: EPA requires the 'pubinfo' element: Publication information.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="//secinfo"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="//secinfo">
					<axsl:attribute name="location">/metadata/idinfo/secinfo</axsl:attribute>
					<svrl:text>Error: EPA requires the 'secinfo' element: Security information.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="//keywords"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="//keywords">
					<axsl:attribute name="location">/metadata/idinfo/keywords</axsl:attribute>
					<svrl:text>Error: EPA requires the 'keywords' element: keywords for EPA and Geo One-stop use.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M6"/>
	</axsl:template>
	<axsl:template match="text()" priority="-1" mode="M6"/>
	<axsl:template match="@*|node()" priority="-2" mode="M6">
		<axsl:choose>
			<!--Housekeeping: SAXON warns if attempting to find the attribute
                           of an attribute-->
			<axsl:when test="not(@*)">
				<axsl:apply-templates select="node()" mode="M6"/>
			</axsl:when>
			<axsl:otherwise>
				<axsl:apply-templates select="@*|node()" mode="M6"/>
			</axsl:otherwise>
		</axsl:choose>
	</axsl:template>

	<!--PATTERN metainfo_err-->


	<!--RULE -->
	<axsl:template match="metainfo//metfrd" priority="4000" mode="M7">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="metainfo//metfrd"/>
		<axsl:variable name="year" select="substring(.,1,4)"/>
		<axsl:variable name="month" select="substring(.,5,2)"/>
		<axsl:variable name="day" select="substring(.,7,2)"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="number($year) &gt; 2006"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="number($year) &gt; 2006">
					<axsl:attribute name="location">/metadata/metainfo/metfrd</axsl:attribute>
					<svrl:text>Error: EPA requires the 'metfrd' element to be in after this checker was introduced, saw '<axsl:text/><axsl:value-of select="$year"/><axsl:text/>' which should be greater than 2006: Metadata Future Review Date.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="translate(.,'0123456789','')='' and (             string-length(.) = 8 or string-length(.) = 6  or string-length(.) = 4)"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="translate(.,'0123456789','')='' and ( string-length(.) = 8 or string-length(.) = 6 or string-length(.) = 4)">
					<axsl:attribute name="location">/metadata/metainfo/metfrd</axsl:attribute>
					<svrl:text>Error: EPA requires the 'metfrd' element to be of the form 'yyyymmdd' or 'yyyymm' or 'yyyy', saw <axsl:text/><axsl:value-of select="."/><axsl:text/> :  Metadata Future Review Date.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>

		<!--ASSERT -->
		<!-- AE 20080908: Took out on request from JZ
		<axsl:choose>
			<axsl:when test="string-length(.) &gt; 4 and number($month) &gt; 0 and number($month) &lt; 13"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="string-length(.) &gt; 4 and number($month) &gt; 0 and number($month) &lt; 13">
					<axsl:attribute name="location">/metadata/metainfo/metfrd</axsl:attribute>
					<svrl:text>
						Error: EPA requires the 'metfrd' element to have month, if present, in the range '1..12', saw a month of '<axsl:text/><axsl:value-of select="$month"/><axsl:text/>' :  Metadata Future Review Date.
					</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
			  -->

		<!--ASSERT -->
		<!-- AE 20080908: Logic dictates this has to be out as well based on the above.
		<axsl:choose>
			<axsl:when test="string-length(.) &gt; 6 and number($day) &gt; 0 and number($day) &lt; 32"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="string-length(.) &gt; 6 and number($day) &gt; 0 and number($day) &lt; 32">
					<axsl:attribute name="location">/metadata/metainfo/metfrd</axsl:attribute>
					<svrl:text>
						Error: EPA requires the 'metfrd' element to have a day-of-month. if present, in '1..31', saw a day of '<axsl:text/><axsl:value-of select="$day"/><axsl:text/>' :  Metadata Future Review Date.
					</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
			  -->
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M7"/>
	</axsl:template>
	<axsl:template match="text()" priority="-1" mode="M7"/>
	<axsl:template match="@*|node()" priority="-2" mode="M7">
		<axsl:choose>
			<!--Housekeeping: SAXON warns if attempting to find the attribute
                           of an attribute-->
			<axsl:when test="not(@*)">
				<axsl:apply-templates select="node()" mode="M7"/>
			</axsl:when>
			<axsl:otherwise>
				<axsl:apply-templates select="@*|node()" mode="M7"/>
			</axsl:otherwise>
		</axsl:choose>
	</axsl:template>

	<!--PATTERN metainfo_glob-->


	<!--RULE -->
	<axsl:template match="metainfo" priority="4000" mode="M8">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="metainfo"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="//metfrd"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="//metfrd">
					<axsl:attribute name="location">/metadata/metainfo/metfrd</axsl:attribute>
					<svrl:text>Error: EPA requires the 'metfrd' element: metadata future review date.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M8"/>
	</axsl:template>
	<axsl:template match="text()" priority="-1" mode="M8"/>
	<axsl:template match="@*|node()" priority="-2" mode="M8">
		<axsl:choose>
			<!--Housekeeping: SAXON warns if attempting to find the attribute
                           of an attribute-->
			<axsl:when test="not(@*)">
				<axsl:apply-templates select="node()" mode="M8"/>
			</axsl:when>
			<axsl:otherwise>
				<axsl:apply-templates select="@*|node()" mode="M8"/>
			</axsl:otherwise>
		</axsl:choose>
	</axsl:template>

	<!--PATTERN req_sections-->


	<!--RULE -->
	<axsl:template match="/" priority="4000" mode="M9">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="/"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="//dataqual"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="//dataqual">
					<axsl:attribute name="location">/metadata/dataqual</axsl:attribute>
					<svrl:text>Error: EPA requires the 'dataqual' element: a general assessment of the quality of the data set.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="//spref"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="//spref">
					<axsl:attribute name="location">/metadata/spref</axsl:attribute>
					<svrl:text>Error: EPA requires the 'spref' element: descriptions of the reference frame for, and the means to encode, coordinates in the data set.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="//distinfo"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="//distinfo">
					<axsl:attribute name="location">/metadata/distinfo</axsl:attribute>
					<svrl:text>Error: EPA requires the 'distinfo' element: information about the distributor of and options for obtaining the data set.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M9"/>
	</axsl:template>
	<axsl:template match="text()" priority="-1" mode="M9"/>
	<axsl:template match="@*|node()" priority="-2" mode="M9">
		<axsl:choose>
			<!--Housekeeping: SAXON warns if attempting to find the attribute
                           of an attribute-->
			<axsl:when test="not(@*)">
				<axsl:apply-templates select="node()" mode="M9"/>
			</axsl:when>
			<axsl:otherwise>
				<axsl:apply-templates select="@*|node()" mode="M9"/>
			</axsl:otherwise>
		</axsl:choose>
	</axsl:template>

	<!--PATTERN ok_values_geodetic-->


	<!--RULE -->
	<axsl:template match="spref/horizsys/geodetic/horizdn" priority="4000" mode="M10">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="spref/horizsys/geodetic/horizdn"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="normalize-space(.) !='' "/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="normalize-space(.) !=''">
					<axsl:attribute name="location">/metadata/spref/horizsys/geodetic/horizdn</axsl:attribute>
					<svrl:text>Error: EPA requires a non-empty 'horizdn' element: Horizontal Datum Name.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M10"/>
	</axsl:template>

	<!--RULE -->
	<axsl:template match="spref/horizsys/geodetic/ellips" priority="3999" mode="M10">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="spref/horizsys/geodetic/ellips"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="normalize-space(.) !='' "/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="normalize-space(.) !=''">
					<axsl:attribute name="location">/metadata/spref/horizsys/geodetic/ellips</axsl:attribute>
					<svrl:text>Error: EPA requires a non-empty 'ellips' element: Ellipsoid Name.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M10"/>
	</axsl:template>

	<!--RULE -->
	<axsl:template match="spref/horizsys/geodetic/semiaxis" priority="3998" mode="M10">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="spref/horizsys/geodetic/semiaxis"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="translate(.,'0123456789.','')='' and number(.) &gt; number('0.0')"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="translate(.,'0123456789.','')='' and number(.) &gt; number('0.0')">
					<axsl:attribute name="location">/metadata/spref/horizsys/geodetic/semiaxis</axsl:attribute>
					<svrl:text>Error: EPA requires the 'semiaxis' element to be a positive number, saw '<axsl:text/><axsl:value-of select="."/><axsl:text/>' : Semi-Major Axis.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M10"/>
	</axsl:template>

	<!--RULE -->
	<axsl:template match="spref/horizsys/geodetic/denflat" priority="3997" mode="M10">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="spref/horizsys/geodetic/denflat"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="translate(.,'0123456789.','')='' and number(.) &gt; 0.0"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="translate(.,'0123456789.','')='' and number(.) &gt; 0.0">
					<axsl:attribute name="location">/metadata/spref/horizsys/geodetic/denflat</axsl:attribute>
					<svrl:text>Error: EPA requires the 'denflat' element to be a positive number, saw '<axsl:text/><axsl:value-of select="."/><axsl:text/>' : Denominator of Flattening Ratio.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M10"/>
	</axsl:template>
	<axsl:template match="text()" priority="-1" mode="M10"/>
	<axsl:template match="@*|node()" priority="-2" mode="M10">
		<axsl:choose>
			<!--Housekeeping: SAXON warns if attempting to find the attribute
                           of an attribute-->
			<axsl:when test="not(@*)">
				<axsl:apply-templates select="node()" mode="M10"/>
			</axsl:when>
			<axsl:otherwise>
				<axsl:apply-templates select="@*|node()" mode="M10"/>
			</axsl:otherwise>
		</axsl:choose>
	</axsl:template>

	<!--PATTERN spref_err-->


	<!--RULE -->
	<axsl:template match="spref/horizsys/geodetic" priority="4000" mode="M11">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="spref/horizsys/geodetic"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="horizdn"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="horizdn">
					<axsl:attribute name="location">/metadata/spref/horizsys/geodetic/horizdn</axsl:attribute>
					<svrl:text>Error: EPA requires a 'horizdn' element: Horizontal Datum Name.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="ellips"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="ellips">
					<axsl:attribute name="location">/metadata/spref/horizsys/geodetic/ellips</axsl:attribute>
					<svrl:text>Error: EPA requires a 'ellips' element: Ellipsoid.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="semiaxis"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="semiaxis">
					<axsl:attribute name="location">/metadata/spref/horizsys/geodetic/semiaxis</axsl:attribute>
					<svrl:text>Error: EPA requires the 'semiaxis' element to be a positive number, saw '<axsl:text/><axsl:value-of select="semiaxis"/><axsl:text/>' : Semi-Major Axis.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="denflat and translate(denflat,'0123456789.','')='' and number(denflat) &gt; 0.0"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="denflat and translate(denflat,'0123456789.','')='' and number(denflat) &gt; 0.0">
					<axsl:attribute name="location">/metadata/spref/horizsys/geodetic/denflat</axsl:attribute>
					<svrl:text>Error: EPA requires the 'denflat' element to be a positive number, saw '<axsl:text/><axsl:value-of select="denflat"/><axsl:text/>' : Denominator of Flattening Ratio.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M11"/>
	</axsl:template>
	<axsl:template match="text()" priority="-1" mode="M11"/>
	<axsl:template match="@*|node()" priority="-2" mode="M11">
		<axsl:choose>
			<!--Housekeeping: SAXON warns if attempting to find the attribute
                           of an attribute-->
			<axsl:when test="not(@*)">
				<axsl:apply-templates select="node()" mode="M11"/>
			</axsl:when>
			<axsl:otherwise>
				<axsl:apply-templates select="@*|node()" mode="M11"/>
			</axsl:otherwise>
		</axsl:choose>
	</axsl:template>

	<!--PATTERN spref_glob-->


	<!--RULE -->
	<axsl:template match="spref" priority="4000" mode="M12">
		<svrl:fired-rule xmlns:svrl="http://purl.oclc.org/dsdl/svrl" context="spref"/>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="//horizsys"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="//horizsys">
					<axsl:attribute name="location">/metadata/spref/horizsys</axsl:attribute>
					<svrl:text>Error: EPA requires the 'horizsys' element: horizontal coordinate system information.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>

		<!--ASSERT -->
		<axsl:choose>
			<axsl:when test="//geodetic"/>
			<axsl:otherwise>
				<svrl:failed-assert xmlns:svrl="http://purl.oclc.org/dsdl/svrl" test="//geodetic">
					<axsl:attribute name="location">/metadata/spref/horizsys/geodetic</axsl:attribute>
					<svrl:text>Error: EPA requires the 'geodetic' element: geodetic model.</svrl:text>
				</svrl:failed-assert>
			</axsl:otherwise>
		</axsl:choose>
		<axsl:apply-templates select="@*|*|comment()|processing-instruction()" mode="M12"/>
	</axsl:template>
	<axsl:template match="text()" priority="-1" mode="M12"/>
	<axsl:template match="@*|node()" priority="-2" mode="M12">
		<axsl:choose>
			<!--Housekeeping: SAXON warns if attempting to find the attribute
                           of an attribute-->
			<axsl:when test="not(@*)">
				<axsl:apply-templates select="node()" mode="M12"/>
			</axsl:when>
			<axsl:otherwise>
				<axsl:apply-templates select="@*|node()" mode="M12"/>
			</axsl:otherwise>
		</axsl:choose>
	</axsl:template>
</axsl:stylesheet>
