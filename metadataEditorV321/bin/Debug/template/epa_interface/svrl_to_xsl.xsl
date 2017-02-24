<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:iso="http://purl.oclc.org/dsdl/schematron"
    xmlns:svrl="http://purl.oclc.org/dsdl/svrl"
    xmlns:xs="jnk" >

	<xsl:namespace-alias stylesheet-prefix="xs" result-prefix="xsl"/>

	<xsl:output indent="yes" method="xml"/>
	<xsl:template match="/">
		<xs:stylesheet version="1.0" exclude-result-prefixes="iso svrl" >

			<xs:template match="/">
				<errs>
					<xsl:apply-templates/>
				</errs>
			</xs:template>
		</xs:stylesheet>
	</xsl:template>

	<xsl:template match="svrl:failed-assert">
		<xsl:variable name="errtype" select = "substring-before(svrl:text,':')" />
		<err>
			<type>
				<xsl:value-of select="$errtype"/>
			</type>
			<message>
				<xsl:value-of select="substring-after(svrl:text,': ')"/>
			</message>
			<linenum></linenum>
			<errid>
				   <!--
				<xsl:if test="$errtype = 'Error' or $errtype = 'Warning'">
				<xsl:value-of select="({@location}/ancestor-or-self::*[@errid])[1]/@errid"/>
				</xsl:if>
							-->
				<xsl:value-of select="@location"/>
			</errid>
		</err>
	</xsl:template>
</xsl:stylesheet>
