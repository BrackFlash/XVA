/*
Copyright (c) 2015 
# Ulf Tomas Bjorklund

# http://twitter.com/ulfbjo

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using PollingDbForUpdates.Core.Model;
using PollingDbForUpdates.Core.Interfaces.Service;
using PollingDbForUpdates.Core.Interfaces.Data;
using PollingDbForUpdates.Core.ViewModel;

namespace PollingDbForUpdates.Service
{ 
	//NOTE:
	//If you need to implement your own logic/code do it in a partial class,
	//modifications in this file may be overwritten.
    public partial class SalesService : BaseService<Sales,SalesViewModel>, ISalesService
    {
		protected new ISalesRepository Repository;				
		
		public SalesService(IUnitOfWork unitOfWork, ISalesRepository salesRepository):base(unitOfWork)
		{
		    base.Repository = Repository = salesRepository;
		}		
		//Implement custom code in a partial class
	}
}