<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output omit-xml-declaration="yes" indent="no" method="text" />
	<xsl:strip-space elements="*"/>
	<xsl:template match="Documents/Document/DocumentHeader">
		<xsl:for-each select="*">
			<xsl:value-of select="." />
			<xsl:text>;</xsl:text>
		</xsl:for-each>
		<xsl:text>
</xsl:text>
	</xsl:template>
	<xsl:template match="Documents/Document/DocumentDetails">
		<xsl:for-each select="*">
			<xsl:value-of select="../../DocumentHeader/DocumentNumber"/>
			<xsl:text>;</xsl:text>
			<xsl:for-each select="*">
				<xsl:value-of select="." />
				<xsl:text>;</xsl:text>
			</xsl:for-each>
			<xsl:text>
</xsl:text>
		</xsl:for-each>
	</xsl:template>

</xsl:stylesheet>