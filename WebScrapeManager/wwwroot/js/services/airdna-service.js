    var airdnaService = {
        ParseUrlForGetCityId: function (url) {
    
            if (url != "") {
                toastr.info('Parse URL starting...')
    
                formService.BlockWorkWithURL()
    
                let data = {
                    url: url
                }
    
                $.post({
                    type: 'POST',
                    url: urls['Airdna_ParseUrlForGetCity'],
                    contentType: 'application/json',
                    data: JSON.stringify(data),
                    success: function (data) {
                        let state = data.state || {}
                        console.log("state:", state)
                        formService.UnBlockWorkWithURL()
    
                        if (state.isOk == true) {
                            toastr.info(`Parsing is complete. ${state.message}`)
                            location.reload()
                        } else {
                            toastr.info(`Parsing had errors. ${state.message}`)
                        }
                    },
                    dataType: 'json'
                })
            } else toastr.info('URL is empty')
        }
    }
