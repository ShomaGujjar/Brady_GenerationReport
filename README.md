# Brady_GenerationReport

An XML file containing generator data (see accompanying file 01-Basic.xml) is produced and provided as input into an input folder on a regular basis. 
The solution automatically pick up the received XML file as soon as it is placed in the input folder (location of input folder is set in a configuration file), perform parsing and data processing as appropriate to achieve the following:
1.	It calculates and output the Total Generation Value for each generator.
2.	It calculates and output the generator with the highest Daily Emissions for each day along with the emission value.
3.	It calculates and output Actual Heat Rate for each coal generator. 
The output is a single XML file in the format as specified by an example accompanying file 01-Basic-Result.xml into an output folder (location of output folder is configured in the Application app.config file). 
