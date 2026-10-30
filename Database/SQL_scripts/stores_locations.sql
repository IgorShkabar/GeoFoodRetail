ALTER TABLE c##gis_user.stores_locations ADD (
    CONSTRAINT fk_stores_loc_chain FOREIGN KEY (chain_id) 
        REFERENCES retail_chains(chain_id) ON DELETE CASCADE,
    CONSTRAINT fk_stores_loc_state FOREIGN KEY (state_code) 
        REFERENCES usa_states_geom(state_code)
);

  