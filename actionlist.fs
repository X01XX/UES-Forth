

: .action-list ( actlst0 -- )
    drop
;

: action-list-deallocate ( actlst0 -- )
    [ ' action-deallocate ] literal over list-apply \ Deallocate action instances in the list.
    list-deallocate                                 \ Deallocate list and links.
;

